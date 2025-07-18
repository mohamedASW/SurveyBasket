using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveryBasket.Api.Data.EnititesConfigurations;

public class VoteAnswerEntityConfiguration : IEntityTypeConfiguration<VoteAnswer>
{
    public void Configure(EntityTypeBuilder<VoteAnswer> builder)
    {
        builder.HasIndex(v=>new {v.VoteId,v.QuestionId}).IsUnique();
    }
}
