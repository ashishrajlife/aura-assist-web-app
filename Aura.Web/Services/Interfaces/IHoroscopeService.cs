using System;
using Aura.Web.Models;

namespace Aura.Web.Services.Interfaces
{
    public interface IHoroscopeService
    {
        Horoscope GetDailyHoroscope(int zodiacId, DateTime date);
    }
}
