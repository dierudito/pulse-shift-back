using dm.PulseShift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace dm.PulseShift.Infra.Data.Mappings;

public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Nome).IsRequired().HasMaxLength(255);
        builder.Property(w => w.Responsavel).HasMaxLength(100);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(w => w.Tipo)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(w => w.SprintReport)
            .WithMany(s => s.WorkItems)
            .HasForeignKey(w => w.SprintReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.ParentWorkItem)
            .WithMany(w => w.SubWorkItems)
            .HasForeignKey(w => w.ParentWorkItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
