using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class NumberSequenceConfiguration : IEntityTypeConfiguration<NumberSequence>
{
    public void Configure(EntityTypeBuilder<NumberSequence> builder)
    {
        builder.ToTable("NumberSequences", t =>
            t.HasCheckConstraint("CK_NumberSequences_LastValue", "\"LastValue\" >= 0"));
        builder.HasKey(s => new { s.Scope, s.Year });
        builder.Property(s => s.Scope).HasMaxLength(64);
    }
}
