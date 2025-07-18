

using Asp.Versioning;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using SurveryBasket.Api.Authentication;
using SurveryBasket.Api.HealthChecks;
using SurveryBasket.Api.Options;
using SurveryBasket.Api.Swagger;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveryBasket.Api;

public static class DependancyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfigurationManager configurationManager)
    {

        services.AddSwaggerconf()
            .AddMapsterConf().
            AddFluentApiconf().
            AddDbcontextService(configurationManager);

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddCors(opt => opt.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyMethod();
            builder.AllowAnyHeader();
        }));
        services.AddHybridCache();
        services.AddDistributedMemoryCache();
        services.AddJwtProviderOptions(configurationManager);
        services.AddHangfireConfig(configurationManager);
        services.Configure<MailSettings>(configurationManager.GetSection(nameof(MailSettings)));
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IDistrabutedCacheService, DistrabutedCacheService>();
        services.AddHttpContextAccessor();
        services.AddHealthChecks().
               AddSqlServer(name: "database", connectionString: configurationManager.GetConnectionString("DefaultConnection")!)
               .AddHangfire(name: "hangfire", setup: x => x.MinimumAvailableServers = 1)
               .AddCheck<EmailServiceCheck>("Email service");
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("iplimiter", (context) =>
            {
                var ip = context.Connection.RemoteIpAddress;
                return RateLimitPartition.GetSlidingWindowLimiter(ip, _ => new SlidingWindowRateLimiterOptions()
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(2),
                    SegmentsPerWindow = 2
                });
            }
            );
            options.AddConcurrencyLimiter("concurrency", configOptions =>
            {
                configOptions.PermitLimit = 1000;
                configOptions.QueueLimit = 100;
                configOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
       new UrlSegmentApiVersionReader(),
       new HeaderApiVersionReader("X-Api-Version"));

        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
        return services;
    }

    private static IServiceCollection AddSwaggerconf(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt => opt.OperationFilter<SwaggerDefaultValues>());
        services.ConfigureOptions<ConfigureSwaggerOptions>();
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
        services.AddDbContext<ApplicationDbcontext>(options =>
        {
            options.UseSqlServer(connectionstr);
        });
        return services;
    }
    private static IServiceCollection AddJwtProviderOptions(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.AddOptions<JwtOptions>().BindConfiguration(nameof(JwtOptions)).ValidateDataAnnotations().ValidateOnStart();

        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
           .AddEntityFrameworkStores<ApplicationDbcontext>().
               AddDefaultTokenProviders();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = jwtOptions!.Issuer,
                ValidAudience = jwtOptions!.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.Key)),

            };

        });
        services.Configure<IdentityOptions>(options =>
        {

            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;

        });

        return services;
    }
    private static IServiceCollection AddHangfireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return services;
    }

}
