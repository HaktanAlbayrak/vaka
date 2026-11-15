using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class CleaningEventConfiguration : IEntityTypeConfiguration<CleaningEvent>
{
    private const string NotLowerHex = "NOT GLOB '*[^0-9a-f]*'";

    public void Configure(EntityTypeBuilder<CleaningEvent> builder)
    {
        builder.ToTable("CleaningEvents", t =>
        {
            t.HasCheckConstraint("CK_CleaningEvents_Sequence", "\"Sequence\" > 0");
            t.HasCheckConstraint("CK_CleaningEvents_PreviousHashPresence",
                "(\"Sequence\" = 1 AND \"PreviousHash\" IS NULL) OR (\"Sequence\" > 1 AND \"PreviousHash\" IS NOT NULL)");
            t.HasCheckConstraint("CK_CleaningEvents_PreviousHashFormat",
                $"\"PreviousHash\" IS NULL OR (length(\"PreviousHash\") = 64 AND \"PreviousHash\" {NotLowerHex})");
            t.HasCheckConstraint("CK_CleaningEvents_HashFormat",
                $"length(\"Hash\") = 64 AND \"Hash\" {NotLowerHex}");
            t.HasCheckConstraint("CK_CleaningEvents_CommandId", "\"CommandId\" IS NULL OR length(\"CommandId\") = 36");
        });
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EventType).IsRequired().HasMaxLength(64);
        builder.Property(e => e.PayloadJson).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.CommandId).HasMaxLength(36);
        builder.Property(e => e.PreviousHash).HasMaxLength(64);
        builder.Property(e => e.Hash).IsRequired().HasMaxLength(64);
        builder.HasIndex(e => new { e.CleaningRecordId, e.Sequence }).IsUnique();
        builder.HasIndex(e => e.ActorUserId);
        builder.HasIndex(e => e.CommandId);
        builder.HasOne(e => e.Record).WithMany(r => r.Events).HasForeignKey(e => e.CleaningRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Actor).WithMany().HasForeignKey(e => e.ActorUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
