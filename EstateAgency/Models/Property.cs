using System.ComponentModel.DataAnnotations;
using RealEstateAgency.Enums;

namespace RealEstateAgency.Models
{
    /// <summary>
    /// Класс, представляющий объект недвижимости.
    /// </summary>
    public class Property
    {
        [Key]
        public int Id { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public PropertyType Type { get; set; }
        public double Area { get; set; }
        public int Rooms { get; set; }
        public int Floor { get; set; }
        public decimal Price { get; set; }
        public PropertyStatus Status { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Метод изменения статуса объекта (согласно диаграмме классов).
        /// </summary>
        /// <param name="nextStatus">Новый статус.</param>
        /// <returns>True, если статус успешно изменен.</returns>
        public bool ChangeStatus(PropertyStatus nextStatus)
        {
            // TODO: Реализовать логику проверки перехода статусов
            Status = nextStatus;
            return true;
        }
    }
}