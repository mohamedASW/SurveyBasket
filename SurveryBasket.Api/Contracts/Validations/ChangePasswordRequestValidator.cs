﻿using SurveryBasket.Api.Consts;

namespace SurveryBasket.Api.Contracts.Validations;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
                .NotEmpty();
        RuleFor(x=>x.NewPassword)
                .NotEmpty()
                .Matches(Expressions.PasswordExpression)
               .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase")
               .NotEqual(x => x.CurrentPassword)
               .WithMessage("New password cannot be same as the current password");

    }
}
