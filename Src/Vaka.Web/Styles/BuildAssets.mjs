import { copyFileSync, mkdirSync, readFileSync, writeFileSync } from "node:fs";

const fontFiles = [
  ["archivo", "archivo-latin-wdth-normal.woff2", "Archivo-Latin.woff2"],
  ["archivo", "archivo-latin-ext-wdth-normal.woff2", "Archivo-LatinExt.woff2"],
  ["hanken-grotesk", "hanken-grotesk-latin-wght-normal.woff2", "HankenGrotesk-Latin.woff2"],
  ["hanken-grotesk", "hanken-grotesk-latin-ext-wght-normal.woff2", "HankenGrotesk-LatinExt.woff2"],
  ["jetbrains-mono", "jetbrains-mono-latin-wght-normal.woff2", "JetBrainsMono-Latin.woff2"],
  ["jetbrains-mono", "jetbrains-mono-latin-ext-wght-normal.woff2", "JetBrainsMono-LatinExt.woff2"],
];

mkdirSync("wwwroot/fonts", { recursive: true });
for (const [pkg, source, target] of fontFiles) {
  copyFileSync(`node_modules/@fontsource-variable/${pkg}/files/${source}`, `wwwroot/fonts/${target}`);
}

const icons = [
  "layout-list", "play", "file-plus", "layout-dashboard", "list-checks", "factory", "clock", "cog",
  "package", "clipboard-list", "users", "chart-column", "settings", "inbox", "lock", "pause", "check",
  "timer-off", "ban", "octagon-x", "circle-dashed", "circle-arrow-right", "circle-slash", "triangle-alert",
  "hourglass", "circle-x", "undo-2", "coffee", "menu", "x", "sun", "moon", "wifi", "wifi-off",
  "arrow-left-right", "plus", "search", "chevron-right", "chevron-left", "chevron-down",
  "panel-left-close", "panel-left-open", "info", "circle-alert", "eye", "eye-off", "droplets",
  "user-round", "funnel", "shield-check", "refresh-cw", "timer", "circle-check", "layers",
  "house", "key-round", "user-plus", "pencil", "user-check", "user-x", "rotate-ccw", "save",
  "arrow-left", "building-2", "calendar-days", "moon-star", "power-off", "route",
  "arrow-up", "arrow-down", "trash-2", "upload", "image", "video", "send", "file-text", "list-ordered", "list",
];

const symbols = icons.map((name) => {
  const svg = readFileSync(`node_modules/lucide-static/icons/${name}.svg`, "utf8");
  const inner = svg.replace(/^[\s\S]*?<svg[^>]*>/, "").replace(/<\/svg>\s*$/, "").trim();
  return `<symbol id="${name}" viewBox="0 0 24 24">${inner}</symbol>`;
});

mkdirSync("wwwroot/lib/icons", { recursive: true });
writeFileSync(
  "wwwroot/lib/icons/Icons.svg",
  `<svg xmlns="http://www.w3.org/2000/svg">\n${symbols.join("\n")}\n</svg>\n`
);
console.log(`${fontFiles.length} yazı tipi, ${icons.length} ikon`);
