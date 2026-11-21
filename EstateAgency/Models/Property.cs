using EstateAgency.States;
using RealEstateAgency.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstateAgency.Models
{
    public class Property
    {
        public int Id { get; set; }

        public Address Address { get; set; } = new();

        public PropertyType Type { get; set; }

        public double Area { get; set; }
        public int Rooms { get; set; }
        public int Floor { get; set; }
        public decimal Price { get; set; }

        /// <summary>Статус — enum, в БД хранится как int.</summary>
        public PropertyStatus Status { get; set; } = PropertyStatus.Free;

        public int RealtorId { get; set; }
        public Realtor Realtor { get; set; } = null!;

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();

        [NotMapped]
        public IPropertyState State => PropertyStates.Resolve(Status);

        [NotMapped]
        public string StateTitle => State.Title;

        [NotMapped]
        public IReadOnlyCollection<PropertyStatus> AvailableTransitions => State.AllowedTransitions;

        public bool ChangeStatus(PropertyStatus next)
        {
            if (!State.CanTransitionTo(next))
                return false;

            Status = next;
            return true;
        }
    }
}