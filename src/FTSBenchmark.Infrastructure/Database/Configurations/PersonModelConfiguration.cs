using FTSBenchmark.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTSBenchmark.Infrastructure.Database.Configurations;

public class PersonModelConfiguration : IEntityTypeConfiguration<PersonModel>
{
    public void Configure(EntityTypeBuilder<PersonModel> builder)
    {
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}