using RealEstateAgency.Enums;
namespace EstateAgency.Models
{
    public class Deal
    {
        public int Id { get; set; }

        public DateTime DealDate { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }

        public DealStatus Status { get; set; } = DealStatus.New;

        // Тип сделки — справочник
        public int DealTypeId { get; set; }
        public DealType DealType { get; set; } = null!;

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        public int RealtorId { get; set; }
        public Realtor Realtor { get; set; } = null!;

        public bool Complete()
        {
            if (Status is DealStatus.Completed or DealStatus.Cancelled)
                return false;

            if (!Property.ChangeStatus(PropertyStatus.Sold))
                return false;

            Status = DealStatus.Completed;
            return true;
        }

        public bool Cancel()
        {
            if (Status is DealStatus.Completed)
                return false;

            if (Property.Status == PropertyStatus.Reserved)
                Property.ChangeStatus(PropertyStatus.Free);

            Status = DealStatus.Cancelled;
            return true;
        }
    }
}