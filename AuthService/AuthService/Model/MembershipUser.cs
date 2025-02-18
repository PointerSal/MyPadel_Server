namespace AuthService.Model
{
    public class MembershipUser
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime MedicalCertificateDate { get; set; }
        public string MedicalCertificatePath { get; set; } // Path in the database
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Municipality { get; set; }
        public string PaymentMethod { get; set; }
        public string Email { get; set; } // New email field to align with Users table

    }

    public class MembershipUserRequest
    {
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime MedicalCertificateDate { get; set; }
        public string MedicalCertificate { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Address { get; set; }
        public string PostalCode { get; set; }
        public string Municipality { get; set; }
        public string PaymentMethod { get; set; }
        public string Email { get; set; } 

    }

    public class FitMembershipRequest
    {
        public string MembershipNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime MedicalCertificateDate { get; set; }
        public string MedicalCertificate { get; set; }
        public string Email { get; set; }

    }


}
