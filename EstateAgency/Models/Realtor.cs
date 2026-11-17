using EstateAgency.Models;

namespace EstateAgency.Models
{
    public class Realtor : User
    {
        public override string GetRoleName() => "Риелтор";

        public override bool CanManageProperties() => true;

        public override bool CanViewAllDeals() => true;

        // Объекты, которые курирует риелтор
        public ICollection<Property> Properties { get; set; } = new List<Property>();

        // Сделки, которые ведёт риелтор
        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }
}