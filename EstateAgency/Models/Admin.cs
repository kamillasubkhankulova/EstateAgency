namespace EstateAgency.Models
{
    public class Admin : User
    {
        public override string GetRoleName() => "Администратор";

        public override bool CanManageProperties() => true;

        public override bool CanViewAllDeals() => true;

        public override bool CanDeleteRecords() => true;
    }
}