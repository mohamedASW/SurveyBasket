using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveryBasket.Api.Data.EnititesConfigurations;

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FName).HasMaxLength(100);
        builder.Property(x => x.LName).HasMaxLength(100);
        builder.OwnsMany(x => x.RefreshTokens, rt =>
        {
            rt.ToTable("RefreshTokens");
            rt.WithOwner().HasForeignKey("UserId");
        });
        builder.OwnsMany(x => x.ChangeEmailTokens, token =>
        {
            token.ToTable("ChangeEmailTokens");
            token.WithOwner().HasForeignKey("UserId");
        });

    }
}
