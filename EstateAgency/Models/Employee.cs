using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace EstateAgency.Models
{
    /// <summary>
    /// Базовый пользователь системы. Наследники: Admin, Realtor, Client.
    /// </summary>
    public abstract class Employee
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Связь с ролью идёт от базового класса
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public bool Authenticate(string password) => PasswordHash == HashPassword(password);

        public abstract string GetRoleName();

        public virtual bool CanManageProperties() => false;

        public virtual bool CanViewAllDeals() => false;

        public virtual bool CanDeleteRecords() => false;

        public static string HashPassword(string password) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }
}