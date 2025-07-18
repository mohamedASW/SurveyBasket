namespace SurveryBasket.Api.Models;

public sealed class Question : AuditLogging
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public int PollId { get; set; }

    public bool IsActive { get; set; } = true;


    public Poll Poll { get; set; } = default!;
    public ICollection<Answer> Answers { get; set; } = [];
    public ICollection<VoteAnswer> VoteAnswers { get; set; } = [];
}
