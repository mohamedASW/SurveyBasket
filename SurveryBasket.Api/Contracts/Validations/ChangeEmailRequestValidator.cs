namespace SurveryBasket.Api.Contracts.Validations;

public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
{
    public ChangeEmailRequestValidator(ApplicationDbcontext context)
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async(request,email ,cancellationtoken) =>
            {
                var isduplicated = await context.Users.AnyAsync((x => x.Email == email), cancellationtoken);
                if (isduplicated)
                    return false;
                return true;
            }).WithMessage("you try to register with used email...!");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty();
    }
}
