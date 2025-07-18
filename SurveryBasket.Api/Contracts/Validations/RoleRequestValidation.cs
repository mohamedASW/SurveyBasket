

namespace SurveryBasket.Api.Contracts.Validations;

public class RoleRequestValidation : AbstractValidator<RoleRequest>
{
    public RoleRequestValidation()
    {
        RuleFor(x => x.Role).NotEmpty().Length(3, 256);

        RuleFor(x => x.Permissions)
           .NotNull()
           .NotEmpty();

        RuleFor(x => x.Permissions)
         .Must(x => x.Distinct(StringComparer.OrdinalIgnoreCase).Count() == x.Count())
         .WithMessage("You cannot add duplicated Permissions for the same question")
         .When(x => x.Permissions != null);

    }

  
}
