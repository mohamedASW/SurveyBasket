namespace SurveryBasket.Api.Contracts.Validations;

public class ConfirmationEmailRequestValidator : AbstractValidator<ConfirmationEmailRequest>
{
    
    public ConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
           RuleFor(x => x.Code)
            .NotEmpty();
         
    }
}
