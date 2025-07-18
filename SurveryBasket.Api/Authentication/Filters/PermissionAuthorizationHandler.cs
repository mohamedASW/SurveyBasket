using SurveryBasket.Api.Consts;

namespace SurveryBasket.Api.Authentication.Filters;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirment>
{
    protected override  Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirment requirement)
    {
        //var user = context.User.Identity;

        //if(user is null || !user.IsAuthenticated)
        //    return;

        //var hasPermission = context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type);

        //if(!hasPermission) 
        //    return;

        if (context.User.Identity is not { IsAuthenticated: true } ||
            !context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type))
            return Task.CompletedTask;

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
