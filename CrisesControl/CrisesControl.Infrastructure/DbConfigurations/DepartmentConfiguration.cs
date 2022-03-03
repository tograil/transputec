using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Department = CrisesControl.Core.Departments.Department;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.Status, "IDX_Status")
            .HasFillFactor(100);

        builder.Property(e => e.DepartmentName).HasMaxLength(100);
    }
}