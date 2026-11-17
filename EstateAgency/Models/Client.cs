using RealEstateAgency.Enums;
using EstateAgency.Models;

namespace EstateAgency.Models
{
    /// <summary>
    /// Клиент — тоже пользователь системы, но с ограниченным доступом:
    /// видит объекты и только свои сделки.
    /// </summary>
    public class Client : User
    {
        public override string GetRoleName() => "Клиент";

        public RequestType RequestType { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Только сделки, где клиент является участником
        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }
}