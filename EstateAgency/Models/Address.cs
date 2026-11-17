using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EstateAgency.Models
{
    /// <summary>Адрес — часть объекта недвижимости (композиция).</summary>
    [Owned]
    public class Address
    {
        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Street { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string House { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Apartment { get; set; } = string.Empty;

        public override string ToString() =>
            string.IsNullOrWhiteSpace(Apartment)
                ? $"{City}, ул. {Street}, д. {House}"
                : $"{City}, ул. {Street}, д. {House}, кв. {Apartment}";
    }
}