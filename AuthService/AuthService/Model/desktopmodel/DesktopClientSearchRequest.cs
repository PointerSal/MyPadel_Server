namespace AuthService.Models.DesktopModel
{
    // Request model for searching the desktop client by email
    public class DesktopClientSearchRequest
    {
        public string Email { get; set; }
    }

    // Request model for updating the desktop client information
    public class DesktopClientUpdateRequest
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Telephone { get; set; }
        public string? PlayerType { get; set; } // +padel or +tennis
        public DateTime? FitCardExpiryDate { get; set; }
        public string? CardNumber { get; set; }
        public string? MedicalCertificate { get; set; }
        public DateTime? MedicalCertificateDate { get; set; }
        public bool? IsVerified { get; set; }

    }

    public class AddUserRequest
    {
        // Fields from RegisterRequest
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public bool IsMarketing { get; set; }
        public string? ProfilePicture { get; set; }

        // Fields from MembershipUser
        public string? CardNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? MedicalCertificateDate { get; set; }
        public string? MedicalCertificatePath { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProvinceOfBirth { get; set; }
        public string? MunicipalityOfBirth { get; set; }
        public string? TaxCode { get; set; }
        public string? Citizenship { get; set; }
        public string? ProvinceOfResidence { get; set; }
        public string? MunicipalityOfResidence { get; set; }
        public string? PostalCode { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public bool IsVerified { get; set; } = false;
    }

}
