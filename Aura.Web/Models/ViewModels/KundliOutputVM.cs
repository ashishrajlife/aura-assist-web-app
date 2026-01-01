using System;
using System.Collections.Generic;

namespace Aura.Web.Models.ViewModels
{
    public class KundliOutputVM
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public TimeSpan TimeOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }

        // Dynamic – no hard coding
        public string Lagna { get; set; }
        public Dictionary<int, List<string>> HousePlanets { get; set; }

        public static KundliOutputVM CreateEmpty(KundliInputVM input)
        {
            var houses = new Dictionary<int, List<string>>();
            for (int i = 1; i <= 12; i++)
                houses[i] = new List<string>();

            return new KundliOutputVM
            {
                FullName = input.FullName,
                DateOfBirth = input.DateOfBirth,
                TimeOfBirth = input.TimeOfBirth,
                PlaceOfBirth = input.PlaceOfBirth,
                Lagna = "Calculating...",
                HousePlanets = houses
            };
        }
    }
}
