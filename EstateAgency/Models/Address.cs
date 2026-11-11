using System.ComponentModel.DataAnnotations;

namespace RealEstateAgency.Models
{
    /// <summary>
    /// Класс, представляющий адрес объекта недвижимости.
    /// </summary>
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
        public override string ToString()
        {
            return $"{City}, ул. {Street}, д. {House}, кв. {Apartment}";
        }
    }
}