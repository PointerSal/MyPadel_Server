namespace AuthService.Model
{
    public class MembershipUser
    {
        public int Id { get; set; }
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
        public string? Email { get; set; }
        public bool IsVerified { get; set; } = false;  
    }

    public class MembershipUserRequest
    {
        public string? CardNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? MedicalCertificateDate { get; set; }
        public string? MedicalCertificate { get; set; }
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
        public string? Email { get; set; }
        public bool IsVerified { get; set; } 

    }

    public class FitMembershipRequest
    {
        public string? MembershipNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? MedicalCertificateDate { get; set; }
        public string? MedicalCertificate { get; set; }
        public string? Email { get; set; }
    }
}
