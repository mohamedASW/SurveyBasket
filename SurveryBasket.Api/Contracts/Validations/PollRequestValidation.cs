

namespace SurveryBasket.Api.Contracts.Validations;

public class PollRequestValidation : AbstractValidator<PollRequest>
{
    public PollRequestValidation()
    {
        RuleFor(x => x.Title).Length(5, 10);
        RuleFor(x => x.Summary).Length(3, 1500);
        RuleFor(x => x.StartsAt).NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.EndsAt).NotEmpty();
        RuleFor(x => x).Must(hasValidEndDate).WithName(nameof(PollRequest.EndsAt))
            .WithMessage("{PropertyName} must be greater than or equal start date");
        
    }

    private bool hasValidEndDate(PollRequest pollRequest)
    {
        return pollRequest.EndsAt>=pollRequest.StartsAt;
    }
}
