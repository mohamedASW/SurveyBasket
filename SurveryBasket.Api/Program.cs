using Asp.Versioning.ApiExplorer;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using SurveryBasket.Api.Seeds;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, configuration) =>
{
   configuration.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddControllers();
builder.Services.AddDependencies(builder.Configuration);
var app = builder.Build();
var scope = app.Services.CreateScope();
var dbconttext = scope.ServiceProvider.GetRequiredService<ApplicationDbcontext>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
var notificationSerivce = scope.ServiceProvider.GetRequiredService<INotificationService>();
var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
await DefaultRoles.SeedDefaultRoles(roleManager);
await DefaultUsers.SeedDefaultUsers(userManager);
await DefaultRoleCliams.SeedDefaultRoleClaims(dbconttext);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"survey basket { desc.GroupName.ToUpperInvariant()}");
        }
    });
    app.UseHangfireDashboard("/jobs", new DashboardOptions()
    {

        Authorization = [
            new HangfireBasicAuthenticationFilter.HangfireCustomBasicAuthenticationFilter{
                User = builder.Configuration.GetValue<string>("HangfireDashboredOptions:username"),
                Pass = builder.Configuration.GetValue<string>("HangfireDashboredOptions:password")
            }
            ]
    });
}
RecurringJob.AddOrUpdate("SendPollNotificationEmail", () => notificationSerivce.SendNewPollNotification(null, default), Cron.Daily);
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.UseRateLimiter();
app.MapHealthChecks("health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
