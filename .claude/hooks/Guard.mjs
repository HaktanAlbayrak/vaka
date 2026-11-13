#!/usr/bin/env node
// PreToolUse kapı bekçisi. Üç kuralı mekanik olarak uygular:
//   1. Commit mesajında Claude etiketi (Co-Authored-By, "Generated with Claude Code") yasak.
//   2. Veritabanında DROP yasak (ham SQL, dotnet ef database drop, migration Up() içinde Drop*).
//   3. ALTER (mevcut tabloyu değiştiren her işlem) kullanıcı onayı ister.
// Ayrıntı: Docs/Process/DbProtocol.md
import { readFileSync, existsSync } from 'node:fs';
import { execSync } from 'node:child_process';
import path from 'node:path';

const DropInMigration = /migrationBuilder\.Drop[A-Z]\w*/;
const AlterInMigration = /migrationBuilder\.(AlterColumn|AlterTable|RenameColumn|RenameTable|RenameIndex|AddColumn|AddForeignKey|AddPrimaryKey|AddUniqueConstraint|AddCheckConstraint)\b/;
const DropInSql = /\bdrop\s+(table|column|index|view|trigger|database|schema)\b/i;
const AlterInSql = /\balter\s+table\b/i;
const SqlContext = /sqlite3|dotnet(\.exe)?\s+ef|ExecuteSql|Invoke-Sqlcmd|\.sql\b|\.db\b/i;
const ClaudeTag = /co-authored-by:\s*claude|noreply@anthropic\.com|generated with\s+\[?claude/i;

function Reply(decision, reason) {
  process.stdout.write(JSON.stringify({
    hookSpecificOutput: {
      hookEventName: 'PreToolUse',
      permissionDecision: decision,
      permissionDecisionReason: reason,
    },
  }));
  process.exit(0);
}

function ExtractUp(source) {
  const start = source.indexOf('void Up(');
  if (start < 0) return source;
  const end = source.indexOf('void Down(', start);
  return end < 0 ? source.slice(start) : source.slice(start, end);
}

function IsMigrationFile(filePath) {
  const normalized = filePath.replace(/\\/g, '/');
  return /\/Migrations\/[^/]+\.cs$/.test(normalized)
    && !/\.Designer\.cs$/.test(normalized)
    && !/ModelSnapshot\.cs$/.test(normalized);
}

function PendingMigrations(projectDir) {
  try {
    const output = execSync('git status --porcelain --untracked-files=all', { cwd: projectDir, encoding: 'utf8' });
    return output.split('\n')
      .map((line) => line.slice(3).trim().replace(/^"|"$/g, ''))
      .filter((file) => file && IsMigrationFile('/' + file))
      .map((file) => path.join(projectDir, file))
      .filter((file) => existsSync(file));
  } catch {
    return [];
  }
}

function ScanMigrations(files) {
  const result = { drop: [], alter: [] };
  for (const file of files) {
    const up = ExtractUp(readFileSync(file, 'utf8'));
    const name = path.basename(file);
    const drop = up.match(DropInMigration) || (DropInSql.test(up) ? ['SQL DROP'] : null);
    if (drop) result.drop.push(`${name} (${drop[0]})`);
    const alter = up.match(AlterInMigration) || (AlterInSql.test(up) ? ['SQL ALTER TABLE'] : null);
    if (alter) result.alter.push(`${name} (${alter[0]})`);
  }
  return result;
}

let input;
try {
  input = JSON.parse(readFileSync(0, 'utf8'));
} catch {
  process.exit(0);
}

const toolName = input.tool_name || '';
const toolInput = input.tool_input || {};
const projectDir = process.env.CLAUDE_PROJECT_DIR || input.cwd || process.cwd();

if (toolName === 'Bash' || toolName === 'PowerShell') {
  const command = String(toolInput.command || '');
  const isCommit = /\bgit\b[^|;&\n]*\bcommit\b/.test(command);
  const isDatabaseUpdate = /dotnet(\.exe)?\s+ef\s+database\s+update/i.test(command);

  if (isCommit && ClaudeTag.test(command)) {
    Reply('deny', 'Commit mesajında Claude etiketi yasak (CLAUDE.md > Commit kuralları). Co-Authored-By ve "Generated with Claude Code" satırlarını çıkar.');
  }
  if (/dotnet(\.exe)?\s+ef\s+database\s+drop/i.test(command)) {
    Reply('deny', 'DROP yasak: "dotnet ef database drop" çalıştırılamaz (Docs/Process/DbProtocol.md).');
  }
  if (SqlContext.test(command) && DropInSql.test(command)) {
    Reply('deny', 'DROP yasak: dbadmin DROP TABLE/COLUMN/INDEX çalıştıramaz (Docs/Process/DbProtocol.md). Gerekliyse kullanıcı elle yapar.');
  }
  if (isCommit || isDatabaseUpdate) {
    const scan = ScanMigrations(PendingMigrations(projectDir));
    if (scan.drop.length) {
      Reply('deny', `DROP yasak: migration Up() içinde Drop işlemi var: ${scan.drop.join(', ')}. Migration'ı eklemeli (expand) olacak şekilde yeniden üret.`);
    }
    if (isDatabaseUpdate && scan.alter.length) {
      Reply('ask', `ALTER onayı gerekli: ${scan.alter.join(', ')} mevcut tabloyu değiştiriyor. DB kaydında (Docs/Db) onay satırı olmalı.`);
    }
  }
  if (SqlContext.test(command) && AlterInSql.test(command)) {
    Reply('ask', 'ALTER onayı gerekli: ALTER TABLE mevcut tabloyu değiştirir (Docs/Process/DbProtocol.md).');
  }
  if (/dotnet(\.exe)?\s+ef\s+migrations\s+remove/i.test(command)) {
    Reply('ask', 'Migration silme onayı gerekli: uygulanmış bir migration silinirse şema ile kod ayrışır.');
  }
  if (/\b(rm|del|Remove-Item)\b[^|;&\n]*\.db\b/i.test(command) && !/[\\/]\.qa[\\/]/.test(command)) {
    Reply('ask', 'Veritabanı dosyası siliniyor (.qa dışı). Bu, tüm tabloları düşürmekle eşdeğerdir; onay gerekli.');
  }
  process.exit(0);
}

if (toolName === 'Write' || toolName === 'Edit') {
  const filePath = String(toolInput.file_path || '');
  const content = String(toolInput.content ?? toolInput.new_string ?? '');
  const normalized = filePath.replace(/\\/g, '/');
  const agentType = input.agent_type || '';
  const isDbOwned = /\/Migrations\//.test(normalized) || /\/Data\/[^/]*DbContext\.cs$/.test(normalized);

  if (agentType && isDbOwned && agentType !== 'dbadmin') {
    Reply('deny', `Bu dosya dbadmin'e aittir (${path.basename(filePath)}). ${agentType} şema dosyalarına yazamaz; backend-engineer DB talebi açar.`);
  }
  if (/\.sql$/i.test(normalized)) {
    if (DropInSql.test(content)) Reply('deny', 'DROP yasak: SQL betiğinde DROP var (Docs/Process/DbProtocol.md).');
    if (AlterInSql.test(content)) Reply('ask', 'ALTER onayı gerekli: SQL betiğinde ALTER TABLE var.');
  }
  if (/\.cs$/i.test(normalized)) {
    const scope = toolName === 'Write' && IsMigrationFile(normalized) ? ExtractUp(content) : content;
    const inMigration = IsMigrationFile(normalized);
    if (DropInSql.test(scope)) Reply('deny', 'DROP yasak: C# içinde ham SQL DROP var (Docs/Process/DbProtocol.md).');
    if (inMigration && toolName === 'Write' && DropInMigration.test(scope)) {
      Reply('deny', 'DROP yasak: migration Up() içinde Drop işlemi yazılamaz.');
    }
    if (AlterInSql.test(scope)) Reply('ask', 'ALTER onayı gerekli: C# içinde ham SQL ALTER TABLE var.');
  }
}

process.exit(0);
