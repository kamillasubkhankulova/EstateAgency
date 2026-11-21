using System.ComponentModel.DataAnnotations;

namespace EstateAgency.Models
{
    /// <summary>Справочник типов сделки: продажа, аренда.</summary>
    public class DealType
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }
}