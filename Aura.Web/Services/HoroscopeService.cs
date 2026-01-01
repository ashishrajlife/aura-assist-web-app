using System;
using System.Linq;
using Aura.Web.Data;
using Aura.Web.Models;
using Aura.Web.Services.Interfaces;

namespace Aura.Web.Services
{
    public class HoroscopeService : IHoroscopeService
    {
        private readonly AuraDbContext _context;

        public HoroscopeService(AuraDbContext context)
        {
            _context = context;
        }

        public Horoscope GetDailyHoroscope(int zodiacId, DateTime date)
        {
            int dailyTypeId = _context.HoroscopeTypes
                .First(x => x.TypeName == "Daily").HoroscopeTypeId;

            return _context.Horoscopes
                .FirstOrDefault(h =>
                    h.ZodiacId == zodiacId &&
                    h.TypeId == dailyTypeId &&
                    h.HoroscopeDate == date.Date);
        }
    }
}
