

namespace SurveryBasket.Api.Contracts.Validations;

public class QuestionRequestValidation : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidation()
    {
        RuleFor(x => x.Content).NotEmpty().Length(3, 1000);

        RuleFor(x => x.Answers)
           .NotNull();

        RuleFor(x => x.Answers)
         .Must(x => x.Count > 1)
         .WithMessage("Question should has at least 2 answers")
         .When(x => x.Answers != null);

        RuleFor(x => x.Answers)
         .Must(x => x.Distinct(StringComparer.OrdinalIgnoreCase).Count() == x.Count)
         .WithMessage("You cannot add duplicated answers for the same question")
         .When(x => x.Answers != null);

    }

  
}
