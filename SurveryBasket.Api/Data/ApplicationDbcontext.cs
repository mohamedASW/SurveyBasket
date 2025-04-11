

namespace SurveryBasket.Api.Data;

public class ApplicationDbcontext : DbContext
{
    public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Poll> Polls { get; set; }
}
