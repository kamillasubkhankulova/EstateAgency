namespace EstateAgency.Models
{
    public class Realtor : Employee
    {
        public override string GetRoleName() => "Риелтор";

        public override bool CanManageProperties() => true;

        public override bool CanViewAllDeals() => true;

        public ICollection<Property> Properties { get; set; } = new List<Property>();

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }
}