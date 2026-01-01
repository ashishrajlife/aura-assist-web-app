using System;
using System.Linq;
using Aura.Web.Data;
using Aura.Web.Models;
using Aura.Web.Services.Interfaces;

namespace Aura.Web.Services
{
    public class ZodiacService : IZodiacService
    {
        private readonly AuraDbContext _context;

        public ZodiacService(AuraDbContext context)
        {
            _context = context;
        }

        public ZodiacSign GetZodiacByDOB(DateTime dob)
        {
            int month = dob.Month;
            int day = dob.Day;

            var zodiacSigns = _context.ZodiacSigns.ToList();

            foreach (var sign in zodiacSigns)
            {
                // Normal range (does NOT cross year)
                if (sign.StartMonth < sign.EndMonth ||
                    (sign.StartMonth == sign.EndMonth && sign.StartDay <= sign.EndDay))
                {
                    if ((month == sign.StartMonth && day >= sign.StartDay) ||
                        (month == sign.EndMonth && day <= sign.EndDay) ||
                        (month > sign.StartMonth && month < sign.EndMonth))
                    {
                        return sign;
                    }
                }
                // Cross-year range (Capricorn)
                else
                {
                    if ((month == sign.StartMonth && day >= sign.StartDay) ||
                        (month == sign.EndMonth && day <= sign.EndDay) ||
                        (month > sign.StartMonth || month < sign.EndMonth))
                    {
                        return sign;
                    }
                }
            }

            return null; // Should never happen if data is correct
        }
    }
}
