namespace SurveryBasket.Api.Contracts.Validations;

public class AccountRequestValidator: AbstractValidator<AccountRequest>
{
    public AccountRequestValidator()
    {
        RuleFor(x=>x.FName)
                .NotEmpty()
                .Length(3,100);
        RuleFor(x=>x.LName)
                .NotEmpty()
                .Length(3,100);
        
    }
}
