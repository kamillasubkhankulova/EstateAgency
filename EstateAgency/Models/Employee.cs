using System.ComponentModel.DataAnnotations;
using RealEstateAgency.Enums;

namespace RealEstateAgency.Models
{
    /// <summary>
    /// Класс, представляющий сотрудника агентства (риелтора, менеджера, админа).
    /// </summary>
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }

        /// <summary>
        /// Метод аутентификации сотрудника (согласно диаграмме классов).
        /// </summary>
        /// <param name="password">Введенный пароль.</param>
        /// <returns>True, если пароль верен.</returns>
        public bool Authenticate(string password)
        {
            // Реализовать проверку хеша
            return false;
        }
    }
}