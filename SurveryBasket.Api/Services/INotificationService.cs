namespace SurveryBasket.Api.Services;

public interface INotificationService
{
    Task SendNewPollNotification(int?pollid = null, CancellationToken cancellationToken = default);
}
