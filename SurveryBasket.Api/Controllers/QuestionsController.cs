namespace SurveryBasket.Api.Controllers;
[Route("api/polls/{pollid}/[controller]")]
[ApiController]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService questionService = questionService;
    [HttpPost]
    [HasPermisson(Permissions.AddQuestions)]
    public async Task<IActionResult> Add(int pollid,QuestionRequest question ,CancellationToken cancellation)
    {
        var result  = await questionService.AddAsync(pollid, question , cancellation);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { pollid, result.Value!.Id }, result.Value)
                                : result.ToProblem();
                               
    }
    [HttpGet]
    [HasPermisson(Permissions.GetQuestions)]
    public async Task<IActionResult> Get(int pollid,CancellationToken cancellation)
    {
        var result = await questionService.GetAllAsync(pollid,cancellation);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("{id}")]
    [HasPermisson(Permissions.GetQuestions)]
    public async Task<IActionResult> Get(int pollid,int id ,CancellationToken cancellation)
    {
        var result = await questionService.GetAsync(pollid,id,cancellation);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPut("{id}")]
    [HasPermisson(Permissions.UpdateQuestions)]
    public async Task<IActionResult> Update(int pollid,int id,QuestionRequest questionRequest,CancellationToken cancellation)
    {
        var result = await questionService.UpdateAsync(pollid, id,questionRequest,cancellation);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpPut("{id}/togglestatus")]
    [HasPermisson(Permissions.UpdateQuestions)]
    public async Task<IActionResult> ToggleStatus(int pollid,int id,CancellationToken cancellation)
    {
        var result = await questionService.ToggleStatusAsync(pollid, id,cancellation);
        return result.IsSuccess ? NoContent() :
             result.ToProblem();
    }
}
