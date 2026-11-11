using System.ComponentModel.DataAnnotations;

namespace RealEstateAgency.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}