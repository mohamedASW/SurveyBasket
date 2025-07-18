using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SurveryBasket.Api.Options;

namespace SurveryBasket.Api.HealthChecks;

public class EmailServiceCheck(IOptionsMonitor<MailSettings> mailSettings) : IHealthCheck
{
    private readonly MailSettings _mailSettings = mailSettings.CurrentValue;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
        using var smtp = new SmtpClient();
        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            return await Task.FromResult(HealthCheckResult.Healthy());

        }
        catch(Exception ex)
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
        }
    }
}
