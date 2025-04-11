﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveryBasket.Api.Data.EnititesConfigurations;

public class PollEntityConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasIndex(x => x.Title).IsUnique();
        builder.Property(x => x.Title).HasMaxLength(50);
        builder.Property(x => x.Summary).HasMaxLength(1500);

    }
}
