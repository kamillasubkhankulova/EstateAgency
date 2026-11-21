using System.ComponentModel.DataAnnotations;

namespace EstateAgency.Models
{
    /// <summary>Справочник ролей: Администратор, Риелтор, Клиент.</summary>
    public class Role
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}