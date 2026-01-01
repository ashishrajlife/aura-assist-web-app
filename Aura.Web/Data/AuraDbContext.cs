using Microsoft.EntityFrameworkCore;
using Aura.Web.Models;

namespace Aura.Web.Data
{
    public class AuraDbContext : DbContext
    {
        public AuraDbContext(DbContextOptions<AuraDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ZodiacSign> ZodiacSigns { get; set; }
        public DbSet<Planet> Planets { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Rashi> Rashis { get; set; }
        public DbSet<Nakshatra> Nakshatras { get; set; }
        public DbSet<Horoscope> Horoscopes { get; set; }
        public DbSet<HoroscopeType> HoroscopeTypes { get; set; }
    }
}
