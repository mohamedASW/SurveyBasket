namespace SurveryBasket.Api.Contracts.Validations;

public class ConfirmationChangingEmailRequestValidator : AbstractValidator<ConfirmationChangingEmailRequest>
{
    public ConfirmationChangingEmailRequestValidator()
    {
       
        RuleFor(x => x.Code)
            .NotEmpty();
        RuleFor(x => x.UserId)
            .NotEmpty();
        
    }
}
