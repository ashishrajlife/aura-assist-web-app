using System;
using Aura.Web.Models;

namespace Aura.Web.Services.Interfaces
{
    public interface IZodiacService
    {
        ZodiacSign GetZodiacByDOB(DateTime dob);
    }
}
