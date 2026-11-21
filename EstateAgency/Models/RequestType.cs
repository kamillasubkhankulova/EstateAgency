using System.ComponentModel.DataAnnotations;

namespace EstateAgency.Models
{
    /// <summary>Справочник типов запроса клиента: покупка, аренда.</summary>
    public class RequestType
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Client> Clients { get; set; } = new List<Client>();
    }
}