namespace SurveryBasket.Api.Contracts.Requests;

public sealed record ResetPasswordRequest
(
   string Email,
   string OTP,
   string NewPassword,
   string ConfirmNewPassword
  
);
