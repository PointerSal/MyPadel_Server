namespace AuthService.Models
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsMarketing { get; set; }
        public string ProfilePicture { get; set; }  

    }

    public class AddPhoneNumberRequest
    {
        public string Email { get; set; }
        public string Cell { get; set; }
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ResendOTPRequest
    {
        public string Email { get; set; }
    }

    public class VerifyEmailRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
    }

    public class VerifyPhoneRequest
    {
        public string Cell { get; set; }
        public string OTP { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

    public class DeleteAccountRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdatePasswordRequest
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class UpdateProfileRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public string ProfilePicture { get; set; }
    }


}
