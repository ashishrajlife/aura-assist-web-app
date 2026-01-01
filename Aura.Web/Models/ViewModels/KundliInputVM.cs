using System;
using System.ComponentModel.DataAnnotations;

namespace Aura.Web.Models.ViewModels
{
    public class KundliInputVM
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan TimeOfBirth { get; set; }

        [Required]
        public string PlaceOfBirth { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }
    }
}
