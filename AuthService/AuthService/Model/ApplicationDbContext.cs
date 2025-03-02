using AuthService.Model.desktopmodel;
using AuthService.Model.Stripe;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Model
{
    public class ApplicationDbContext : DbContext
    {
        //checkpush
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MembershipUser> MembershipUsers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<PriceTbl> PriceTbls { get; set; }
        public DbSet<FitMembershipTbl> FitMembershipTbls { get; set; }

        public DbSet<CourtSports> CourtSports { get; set; }
        public DbSet<Refund> Refunds { get; set; }


    }
}
