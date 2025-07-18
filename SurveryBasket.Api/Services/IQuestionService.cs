namespace SurveryBasket.Api.Services;

public interface IQuestionService
{
    Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest questionRequest, CancellationToken cancellation);
    Task<Result<QuestionResponse>> GetAsync(int pollid , int questionid , CancellationToken cancellation);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollid ,string userId, CancellationToken cancellation);
    
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollid,CancellationToken cancellation);
    Task<Result> UpdateAsync(int pollid, int questionid, QuestionRequest questionRequest, CancellationToken cancellation);
    Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);
}
