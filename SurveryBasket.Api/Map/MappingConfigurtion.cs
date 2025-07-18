namespace SurveryBasket.Api.Map;
public class MappingConfigurtion: IRegister
{
   
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>().
             Map(x => x.Answers, s => s.Answers.Select(answer => new Answer { Content = answer }));
       
        config.NewConfig<RegistrationRequest, ApplicationUser>()
            .Map(dst => dst.UserName, src => src.email)
            .Map(dst => dst.Email, src => src.email)
            .Map(dst => dst.FName, src => src.firstname)
            .Map(dst => dst.LName, src => src.lastname);

        config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
            .Map(dst=>dst , src=>src.user)
            .Map(dst => dst.FirstName, src => src.user.FName)
            .Map(dst => dst.LastName, src => src.user.LName)
            .Map(dst => dst.Roles, src => src.roles);

        config.NewConfig<UserRequest, ApplicationUser>()
            .Map(dst => dst.UserName, src => src.Email)
            .Map(dst => dst.EmailConfirmed,_=>true);
        config.NewConfig<UpdateUserRequest, ApplicationUser>()
            .Map(dst => dst.UserName, src => src.Email);





    }
}
