

using SurveryBasket.Api.Data;

namespace SurveryBasket.Api;

public static class DependancyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,IConfigurationManager configurationManager)
    {

        services.AddSwaggerconf()
            .AddMapsterConf().
            AddFluentApiconf().
            AddDbcontextService(configurationManager);

        services.AddScoped<IPollService, PollService>();
        return services;
    }

    private static IServiceCollection AddSwaggerconf(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
    private static IServiceCollection AddMapsterConf(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MappingConfigurtion).Assembly);
        services.AddSingleton<IMapper>(new Mapper(config));
        return services;
    }
    private static IServiceCollection AddFluentApiconf(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation().AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    private static IServiceCollection AddDbcontextService(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        var connectionstr = configurationManager.GetConnectionString("DefaultConnection") ?? throw new InvalidDataException("invalid connection string with this name !");
        services.AddDbContext<ApplicationDbcontext>(options => {
            options.UseSqlServer(connectionstr);
        });
        return services;
    }
}
