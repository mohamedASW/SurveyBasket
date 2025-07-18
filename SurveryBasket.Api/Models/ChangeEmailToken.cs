namespace SurveryBasket.Api.Models;

public class ChangeEmailToken
{
    public string  Token { get; set; }  = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedOn { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpireDate;
    public bool IsActive => RevokedOn is null && !IsExpired;

}
