

namespace SurveryBasket.Api.Contracts.Validations;

public class RefreshTokenRequestValidation : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidation()
    {
        RuleFor(x => x.token)
            .NotEmpty();
        RuleFor(x => x.refreshtoken)
            .NotEmpty();

    }

   
}
