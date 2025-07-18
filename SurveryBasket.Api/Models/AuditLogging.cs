

using System.ComponentModel.DataAnnotations.Schema;

namespace SurveryBasket.Api.Models;

public class AuditLogging
{
   
    public string CreatedById { get; set; } =string.Empty;
    
    public string? UpdatedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public ApplicationUser CreatedBy { get; set; } = default!;
    [ForeignKey(nameof(UpdatedById))]
    public ApplicationUser UpdatedBy { get; set; }  = default!;
}
