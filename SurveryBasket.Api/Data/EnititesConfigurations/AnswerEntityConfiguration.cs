using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveryBasket.Api.Consts;

namespace SurveryBasket.Api.Data.EnititesConfigurations;

public class AnswerEntityConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasIndex(a=>new {a.QuestionId,a.Content}).IsUnique();
        
        builder.Property(x => x.Content).HasMaxLength(1500);

    }
}
