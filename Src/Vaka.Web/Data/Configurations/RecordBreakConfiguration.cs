using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class RecordBreakConfiguration : IEntityTypeConfiguration<RecordBreak>
{
    public void Configure(EntityTypeBuilder<RecordBreak> builder)
    {
        builder.ToTable("RecordBreaks", t =>
        {
            t.HasCheckConstraint("CK_RecordBreaks_EndedAfterStart", "\"EndedAtUtc\" IS NULL OR \"EndedAtUtc\" >= \"StartedAtUtc\"");
            t.HasCheckConstraint("CK_RecordBreaks_EndKind", "\"EndKind\" IS NULL OR \"EndKind\" IN (1, 2, 3)");
            t.HasCheckConstraint("CK_RecordBreaks_EndFieldsTogether",
                "(\"EndedAtUtc\" IS NULL) = (\"EndKind\" IS NULL) AND (\"EndedAtUtc\" IS NULL) = (\"EndedByUserId\" IS NULL)");
        });
        builder.HasKey(b => b.Id);
        builder.Property(b => b.EndKind).HasConversion<int?>();
        builder.Property(b => b.EndReason).HasMaxLength(500);
        builder.HasIndex(b => new { b.CleaningRecordId, b.StartedAtUtc });
        builder.HasIndex(b => b.DuringStepId);
        builder.HasIndex(b => b.EndedByUserId);
        builder.HasIndex(b => b.CleaningRecordId, "IX_RecordBreaks_OneOpenPerRecord").IsUnique().HasFilter("\"EndedAtUtc\" IS NULL");
        builder.HasIndex(b => b.StartedByUserId, "IX_RecordBreaks_OneOpenPerUser").IsUnique().HasFilter("\"EndedAtUtc\" IS NULL");
        builder.HasOne(b => b.Record).WithMany(r => r.Breaks).HasForeignKey(b => b.CleaningRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.StartedBy).WithMany().HasForeignKey(b => b.StartedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.EndedBy).WithMany().HasForeignKey(b => b.EndedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.DuringStep).WithMany().HasForeignKey(b => b.DuringStepId).OnDelete(DeleteBehavior.Restrict);
    }
}
