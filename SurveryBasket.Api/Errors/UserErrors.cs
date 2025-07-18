﻿
namespace SurveryBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

    public static readonly Error DisabledUser =
        new("User.DisabledUser", "Disabled user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error LockedUser =
        new("User.LockedUser", "Locked user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error UserNotFound =
        new("User.UserNotFound", "User is not found", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedEmail =
       new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);


    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Error NotConfirmedEmail =
        new("User.NotConfirmedEmail", "you did not confirm email", StatusCodes.Status401Unauthorized);
    public static readonly Error DuplicatedConfirmedEmail =
        new("User.DuplicatedConfirmedEmail", "you try to confirm email twice", StatusCodes.Status409Conflict);
    public static readonly Error InvalidCode =
        new("User.InvalidCode", "invalid code", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidRoles =
       new("User.InvalidRoles", "Invalid roles", StatusCodes.Status400BadRequest);
}
