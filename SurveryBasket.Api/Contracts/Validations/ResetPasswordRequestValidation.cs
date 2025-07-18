using SurveryBasket.Api.Consts;

namespace SurveryBasket.Api.Contracts.Validations;

public class ResetPasswordRequestValidation : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();
        RuleFor(x=>x.OTP).NotEmpty();
        RuleFor(x=>x.NewPassword)
            .Matches(Expressions.PasswordExpression)
            .NotEmpty();
        RuleFor(x=>x.ConfirmNewPassword)
            .NotEmpty()
            .Equal(x=>x.NewPassword);
    }
}
