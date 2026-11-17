using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace EstateAgency.Models
{
    /// <summary>
    /// Базовый пользователь системы. Наследники: Admin, Realtor, Client.
    /// </summary>
    public abstract class User
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

        public bool Authenticate(string password) => PasswordHash == HashPassword(password);

        /// <summary>Название роли для интерфейса.</summary>
        public abstract string GetRoleName();

        /// <summary>Может добавлять и редактировать объекты.</summary>
        public virtual bool CanManageProperties() => false;

        /// <summary>Может просматривать сделки всех пользователей.</summary>
        public virtual bool CanViewAllDeals() => false;

        /// <summary>Может удалять записи (только администратор).</summary>
        public virtual bool CanDeleteRecords() => false;

        public static string HashPassword(string password) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }
}