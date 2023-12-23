using FTSBenchmark.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTSBenchmark.Infrastructure.Postgres.Configurations;

public class PersonPostgresModelConfiguration : IEntityTypeConfiguration<PersonModel>
{
    public void Configure(EntityTypeBuilder<PersonModel> builder)
    {
        builder.ToTable("persons");
       
        builder.HasKey(e => e.Id).HasName("persons_pkey");
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.FirstName)
            .HasMaxLength(255)
            .HasColumnName("firstname");
        builder.Property(e => e.LastName)
            .HasMaxLength(255)
            .HasColumnName("lastname");
        builder.Property(e => e.Username)
            .HasMaxLength(255)
            .HasColumnName("username");

        builder.HasIndex(e => e.FirstName, "first_name_trigram_idx")
            .HasMethod("gin")
            .HasOperators(new[] { "gin_trgm_ops" });

        builder.HasIndex(e => e.LastName, "last_name_trigram_idx")
            .HasMethod("gin")
            .HasOperators(new[] { "gin_trgm_ops" });
    }
}