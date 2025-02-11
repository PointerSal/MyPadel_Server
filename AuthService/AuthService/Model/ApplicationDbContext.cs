using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MembershipUser> MembershipUsers { get; set; }
        public DbSet<Booking> Bookings { get; set; }  // Added DbSet for Booking
    }
}
