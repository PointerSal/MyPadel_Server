using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Cell { get; set; }
        public string? PhoneOTP { get; set; }
        public DateTime? PhoneOTPExpiry { get; set; }

        public string? EmailOTP { get; set; }  
        public DateTime? EmailOTPExpiry { get; set; }  

        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsActive { get; set; }
    }
}
