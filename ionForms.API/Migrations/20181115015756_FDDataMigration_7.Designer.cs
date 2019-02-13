﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ionForms.API.Entities;

namespace ionForms.API.Migrations
{
    [DbContext(typeof(FDDataContext))]
    [Migration("20181115015756_FDDataMigration_7")]
    partial class FDDataMigration_7
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ionForms.API.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(80);

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("ionForms.API.Entities.Column", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ColumnLength")
                        .HasMaxLength(20);

                    b.Property<string>("ColumnName")
                        .IsRequired()
                        .HasMaxLength(80);

                    b.Property<string>("ColumnType")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ColumnValue");

                    b.Property<string>("DefaultValue");

                    b.Property<string>("Description")
                        .HasMaxLength(5000);

                    b.Property<string>("DisplayName")
                        .HasMaxLength(500);

                    b.Property<int>("FormId");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsRequired");

                    b.HasKey("Id");

                    b.HasIndex("FormId");

                    b.ToTable("Columns");
                });

            modelBuilder.Entity("ionForms.API.Entities.Form", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(80);

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("ionForms.API.Entities.Column", b =>
                {
                    b.HasOne("ionForms.API.Entities.Form", "Form")
                        .WithMany("Columns")
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ionForms.API.Entities.Form", b =>
                {
                    b.HasOne("ionForms.API.Entities.Account", "Account")
                        .WithMany("Forms")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}