using aura_assist_prod.Models;
using Microsoft.EntityFrameworkCore;

namespace aura_assist_prod.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InfluencerProfile> InfluencerProfiles { get; set; }
        public DbSet<AgencyProfile> AgencyProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraint for email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure relationships
            modelBuilder.Entity<InfluencerProfile>()
                .HasOne(ip => ip.User)
                .WithOne(u => u.InfluencerProfile)
                .HasForeignKey<InfluencerProfile>(ip => ip.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AgencyProfile>()
                .HasOne(ap => ap.User)
                .WithOne(u => u.AgencyProfile)
                .HasForeignKey<AgencyProfile>(ap => ap.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
