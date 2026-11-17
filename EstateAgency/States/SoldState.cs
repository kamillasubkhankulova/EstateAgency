using RealEstateAgency.Enums;

namespace EstateAgency.States
{
    /// <summary>Продан: конечное состояние, переходов нет.</summary>
    public sealed class SoldState : IPropertyState
    {
        public static readonly SoldState Instance = new();

        private static readonly PropertyStatus[] Allowed = Array.Empty<PropertyStatus>();

        private SoldState() { }

        public PropertyStatus Status => PropertyStatus.Sold;

        public string Title => "Продан";

        public IReadOnlyCollection<PropertyStatus> AllowedTransitions => Allowed;

        public bool CanTransitionTo(PropertyStatus next) => false;
    }
}