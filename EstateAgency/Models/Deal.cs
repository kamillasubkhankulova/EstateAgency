using System;
using System.ComponentModel.DataAnnotations;
using RealEstateAgency.Enums;

namespace RealEstateAgency.Models
{
    /// <summary>
    /// Класс, представляющий сделку (продажу или аренду).
    /// </summary>
    public class Deal
    {
        [Key]
        public int Id { get; set; }
        public DateTime DealDate { get; set; }
        public decimal Amount { get; set; }
        public DealType Type { get; set; }
        public DealStatus Status { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Метод завершения сделки 
        /// </summary>
        /// <returns>True, если сделка завершена.</returns>
        public bool Complete()
        {
            Status = DealStatus.Completed;
            return true;
        }

        /// <summary>
        /// Метод отмены сделки 
        /// </summary>
        /// <returns>True, если сделка отменена.</returns>
        public bool Cancel()
        {
            Status = DealStatus.Cancelled;
            return true;
        }
    }
}