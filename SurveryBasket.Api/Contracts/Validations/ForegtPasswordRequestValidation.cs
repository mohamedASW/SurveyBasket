namespace SurveryBasket.Api.Contracts.Validations;

public class ForgetPasswordRequestValidation : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();
    }
}
