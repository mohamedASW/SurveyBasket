namespace SurveryBasket.Api.Contracts.Validations;

public class ConfirmationNewEmailRequestValidator : AbstractValidator<ConfirmationNewEmailRequest>
{
    public ConfirmationNewEmailRequestValidator(ApplicationDbcontext context)
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty();
        RuleFor(x => x.Code)
            .NotEmpty();
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
