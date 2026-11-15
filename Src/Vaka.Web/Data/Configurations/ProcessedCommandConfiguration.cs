using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class ProcessedCommandConfiguration : IEntityTypeConfiguration<ProcessedCommand>
{
    public void Configure(EntityTypeBuilder<ProcessedCommand> builder)
    {
        builder.ToTable("ProcessedCommands", t =>
            t.HasCheckConstraint("CK_ProcessedCommands_CommandId", "length(\"CommandId\") = 36"));
        builder.HasKey(c => c.CommandId);
        builder.Property(c => c.CommandId).HasMaxLength(36);
        builder.Property(c => c.CommandType).IsRequired().HasMaxLength(64);
        builder.Property(c => c.ResultJson).IsRequired().HasMaxLength(2000);
        builder.HasIndex(c => c.UserId);
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
