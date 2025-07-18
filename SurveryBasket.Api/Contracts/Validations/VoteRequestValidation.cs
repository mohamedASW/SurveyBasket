namespace SurveryBasket.Api.Contracts.Validations;

public class VoteRequestValidation : AbstractValidator<VoteRequest>
{
    public VoteRequestValidation()
    {
        RuleFor(x => x.VoteRequestAnswers)
            .NotEmpty();
        RuleFor(x => x.VoteRequestAnswers)
            .Must(x=>x.DistinctBy(x=>x.QuestionId).Count()==x.Count()).WithMessage("you tried to vote question twice ...!").When(x=>x.VoteRequestAnswers is not null);
        
        RuleForEach(x => x.VoteRequestAnswers).SetValidator(new VoteRequestAnswerValidation());
    }
}
public class VoteRequestAnswerValidation : AbstractValidator<VoteRequestAnswer>
{
    
    public VoteRequestAnswerValidation()
    {
     
        RuleFor(x => x.QuestionId)
            .GreaterThan(0);
        RuleFor(x => x.AnswerId)
            .GreaterThan(0);
      
        
    }
}
