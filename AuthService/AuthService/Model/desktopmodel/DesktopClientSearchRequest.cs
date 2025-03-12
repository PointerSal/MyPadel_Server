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
        public bool? IsVerified { get; set; }

    }
    

}
