﻿// <auto-generated />
using FTSBenchmark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FTSBenchmark.Infrastructure.Database.Migrations
{
    [DbContext(typeof(BenchmarkDbContext))]
    partial class BenchmarkDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("FTSBenchmark.Domain.PersonModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("FirstName", "LastName")
                        .HasDatabaseName("idx_fulltext")
                        .HasAnnotation("MySql:FullTextIndex", true);

                    b.ToTable("Persons");
                });
#pragma warning restore 612, 618
        }
    }
}
