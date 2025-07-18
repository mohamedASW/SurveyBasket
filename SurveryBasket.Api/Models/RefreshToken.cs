namespace SurveryBasket.Api.Models;

public class RefreshToken
{
    public string  Token { get; set; }  = string.Empty;
    public DateTime ExpireDate { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedOn { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpireDate;
    public bool IsActive => RevokedOn is null && !IsExpired;

}
