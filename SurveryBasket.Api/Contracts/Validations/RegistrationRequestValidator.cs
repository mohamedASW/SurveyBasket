using SurveryBasket.Api.Consts;

namespace SurveryBasket.Api.Contracts.Validations;

public class RegistrationRequestValidator:AbstractValidator<RegistrationRequest>
{
    public RegistrationRequestValidator(ApplicationDbcontext context)
    {
        RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async(request,email ,cancellationtoken) =>
            {
                var isduplicated = await context.Users.AnyAsync((x => x.Email == email), cancellationtoken);
                if (isduplicated)
                    return false;
                return true;
            }).WithMessage("you try to register with used email...!");

        RuleFor(x => x.password)
            .NotEmpty().Matches(Expressions.PasswordExpression);
            
        RuleFor(x=>x.firstname)
            .NotEmpty()
            .Length(3,100);
        RuleFor(x=>x.lastname)
            .NotEmpty()
            .Length(3,100);
        
    }
}
