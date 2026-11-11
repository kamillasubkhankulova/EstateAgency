using System;
using System.ComponentModel.DataAnnotations;
using RealEstateAgency.Enums;

namespace RealEstateAgency.Models
{
    /// <summary>
    /// Класс, представляющий клиента агентства недвижимости.
    /// </summary>
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public RequestType RequestType { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}