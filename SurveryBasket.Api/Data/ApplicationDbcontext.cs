
using Microsoft.AspNetCore.Identity;
using SurveryBasket.Api.Consts;
using System.Security.Claims;

namespace SurveryBasket.Api.Data;

public class ApplicationDbcontext : IdentityDbContext<ApplicationUser , ApplicationRole ,string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options , IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var foreignkeys = modelBuilder.Model.GetEntityTypes()
               .SelectMany(t => t.GetForeignKeys())
                .Where(f => f.DeleteBehavior == DeleteBehavior.Cascade && !f.IsOwnership);
        foreach (var foreignKey in foreignkeys)
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
      
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditLogging>();
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        foreach (var entityentry in entries)
        {
            if (entityentry.State == EntityState.Added)
            {

                entityentry.Entity.CreatedById = userId!;
            }
            else if (entityentry.State == EntityState.Modified)
            {
                entityentry.Entity.UpdatedAt = DateTime.UtcNow;
                entityentry.Entity.UpdatedById =userId!;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<Question>Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteAnswer> VoteAnswers { get; set; }
    
}
