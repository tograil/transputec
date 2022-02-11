﻿using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyMessageTransactionStatConfiguration : IEntityTypeConfiguration<CompanyMessageTransactionStat>
{
    public void Configure(EntityTypeBuilder<CompanyMessageTransactionStat> builder)
    {
        builder.HasNoKey();

        builder.ToView("CompanyMessageTransactionStats");
    }
}