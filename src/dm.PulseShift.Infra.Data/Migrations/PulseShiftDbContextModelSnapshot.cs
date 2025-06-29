﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dm.PulseShift.Infra.Data.Context;

#nullable disable

namespace dm.PulseShift.Infra.Data.Migrations
{
    [DbContext(typeof(PulseShiftDbContext))]
    partial class PulseShiftDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.0-preview.5.25277.114")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.Activity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CardCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CardLink")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CardCode")
                        .IsUnique()
                        .HasDatabaseName("IX_Activities_CardCode");

                    b.ToTable("Activities", (string)null);
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.ActivityPeriod", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ActivityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssociatedEndTimeEntryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssociatedStartTimeEntryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("AssociatedEndTimeEntryId");

                    b.HasIndex("AssociatedStartTimeEntryId");

                    b.ToTable("ActivityPeriods", (string)null);
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.DayOff", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Date")
                        .IsUnique();

                    b.ToTable("DaysOff", (string)null);
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.SprintReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataFim")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataInicio")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("NomeSprint")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("NumeroSprint")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("SprintReports");
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.TimeEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("EntryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EntryType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateOnly>("WorkDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("TimeEntries", (string)null);
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.WorkItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid?>("ParentWorkItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Responsavel")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("SprintReportId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ParentWorkItemId");

                    b.HasIndex("SprintReportId");

                    b.ToTable("WorkItems");
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.WorkSchedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("EntryType")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<TimeSpan>("WorkTime")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("EntryType")
                        .IsUnique();

                    b.ToTable("WorkSchedules", (string)null);
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.ActivityPeriod", b =>
                {
                    b.HasOne("dm.PulseShift.Domain.Entities.Activity", "Activity")
                        .WithMany("ActivityPeriods")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dm.PulseShift.Domain.Entities.TimeEntry", "EndTimeEntry")
                        .WithMany("EndActivityPeriods")
                        .HasForeignKey("AssociatedEndTimeEntryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("dm.PulseShift.Domain.Entities.TimeEntry", "StartTimeEntry")
                        .WithMany("StartActivityPeriods")
                        .HasForeignKey("AssociatedStartTimeEntryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("EndTimeEntry");

                    b.Navigation("StartTimeEntry");
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.WorkItem", b =>
                {
                    b.HasOne("dm.PulseShift.Domain.Entities.WorkItem", "ParentWorkItem")
                        .WithMany("SubWorkItems")
                        .HasForeignKey("ParentWorkItemId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("dm.PulseShift.Domain.Entities.SprintReport", "SprintReport")
                        .WithMany("WorkItems")
                        .HasForeignKey("SprintReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentWorkItem");

                    b.Navigation("SprintReport");
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.Activity", b =>
                {
                    b.Navigation("ActivityPeriods");
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.SprintReport", b =>
                {
                    b.Navigation("WorkItems");
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.TimeEntry", b =>
                {
                    b.Navigation("EndActivityPeriods");

                    b.Navigation("StartActivityPeriods");
                });

            modelBuilder.Entity("dm.PulseShift.Domain.Entities.WorkItem", b =>
                {
                    b.Navigation("SubWorkItems");
                });
#pragma warning restore 612, 618
        }
    }
}
