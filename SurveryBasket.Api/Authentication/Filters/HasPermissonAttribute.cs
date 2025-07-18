namespace SurveryBasket.Api.Authentication.Filters;

public class HasPermissonAttribute(string permission) : AuthorizeAttribute(permission)
{

}
