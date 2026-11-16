using Microsoft.EntityFrameworkCore;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data;

public class VakaDbContext : DbContext
{
    public VakaDbContext(DbContextOptions<VakaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<SettingChange> SettingChanges => Set<SettingChange>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<MaterialLot> MaterialLots => Set<MaterialLot>();
    public DbSet<CleaningType> CleaningTypes => Set<CleaningType>();
    public DbSet<MachineCleaningType> MachineCleaningTypes => Set<MachineCleaningType>();
    public DbSet<ProcedureVersion> ProcedureVersions => Set<ProcedureVersion>();
    public DbSet<ProcedurePhase> ProcedurePhases => Set<ProcedurePhase>();
    public DbSet<ProcedureStep> ProcedureSteps => Set<ProcedureStep>();
    public DbSet<StepMedia> StepMedia => Set<StepMedia>();
    public DbSet<CleaningRecord> CleaningRecords => Set<CleaningRecord>();
    public DbSet<RecordPhase> RecordPhases => Set<RecordPhase>();
    public DbSet<RecordStep> RecordSteps => Set<RecordStep>();
    public DbSet<StepParticipant> StepParticipants => Set<StepParticipant>();
    public DbSet<RecordMaterial> RecordMaterials => Set<RecordMaterial>();
    public DbSet<CleaningEvent> CleaningEvents => Set<CleaningEvent>();
    public DbSet<ProcessedCommand> ProcessedCommands => Set<ProcessedCommand>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();
    public DbSet<RecordBreak> RecordBreaks => Set<RecordBreak>();
    public DbSet<StepWorkInterval> StepWorkIntervals => Set<StepWorkInterval>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VakaDbContext).Assembly);
    }
}
