using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveryBasket.Api.Data.EnititesConfigurations;

public class VoteEntityConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasIndex(v=>new {v.PollId,v.UserId}).IsUnique();
     
    }
}
