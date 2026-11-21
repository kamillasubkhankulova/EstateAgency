namespace EstateAgency.Models
{
    public class Client : Employee
    {
        public override string GetRoleName() => "Клиент";

        // В TPH колонка общая для всех сотрудников, поэтому допускаем NULL
        public int? RequestTypeId { get; set; }
        public RequestType? RequestType { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }
}