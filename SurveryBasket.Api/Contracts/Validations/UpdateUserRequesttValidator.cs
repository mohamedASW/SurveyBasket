namespace SurveryBasket.Api.Contracts.Validations;

public class UpdateUserRequesttValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequesttValidator(ApplicationDbcontext context)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x=>x.FName)
            .Length(3,100);
        RuleFor(x=>x.LName)
            .Length(3,100);
        RuleFor(x=>x.Roles)
            .NotEmpty();
        RuleFor(x => x.Roles)
            .Must(x => x.Distinct(StringComparer.OrdinalIgnoreCase).Count() == x.Count()).
             WithMessage("You cannot add duplicated Role for the same User").
            When(x => x.Roles!=null);

        
    }
}
