using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveryBasket.Api.Data.EnititesConfigurations;

public class QuestionEntityConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(q=>new {q.PollId,q.Content}).IsUnique();
        
        builder.Property(x => x.Content).HasMaxLength(1500);

    }
}
