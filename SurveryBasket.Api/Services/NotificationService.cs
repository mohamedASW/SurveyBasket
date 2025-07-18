
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Helpers;

namespace SurveryBasket.Api.Services;

public class NotificationService(ApplicationDbcontext dbContext,
    UserManager<ApplicationUser> userManager ,
    IEmailSender emailSender) : INotificationService
{
    private readonly ApplicationDbcontext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;
    public async Task SendNewPollNotification(int? pollid =null, CancellationToken cancellationToken = default)
    {
        IList<Poll> polls = [];
        if (pollid.HasValue)
        {
            var poll  = _dbContext.Polls.AsNoTracking().SingleOrDefault(x=>x.Id.Equals(pollid.Value)&&x.IsPublished);
            polls.Add(poll!);
        }
        else
        {
            polls = await _dbContext.Polls.AsNoTracking().Where(x => x.IsPublished && x.StartsAt.Equals(DateOnly.FromDateTime(DateTime.UtcNow))).ToListAsync(cancellationToken);
        }
        var users = await _userManager.GetUsersInRoleAsync(AppRoles.Member);       
        foreach(var user in users)
        {
            foreach (var poll in polls)
            {
                var placeholders = new Dictionary<string, string>
                {
                    { "{{name}}", user.FName },
                    { "{{pollTill}}", poll.Title },
                    { "{{endDate}}", poll.EndsAt.ToString() },
                    { "{{url}}", $"/polls/start/{poll.Id}" }
                };
                var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholders);

                await _emailSender.SendEmailAsync(user.Email!, $"📣 Survey Basket: New Poll - {poll.Title}", body);
            }
        }
    }
}
