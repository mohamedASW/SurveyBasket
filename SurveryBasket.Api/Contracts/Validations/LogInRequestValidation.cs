

using Org.BouncyCastle.Bcpg.Sig;

namespace SurveryBasket.Api.Contracts.Validations;

public class LogInRequestValidation : AbstractValidator<LogInRequest>
{
    public LogInRequestValidation()
    {
        RuleFor(x => x.email)
            .NotEmpty().EmailAddress();
        RuleFor(x => x.password)
            .NotEmpty();

    }

   
}
