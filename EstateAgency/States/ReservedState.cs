using RealEstateAgency.Enums;

namespace EstateAgency.States
{
    /// <summary>Забронирован: сделку можно завершить или снять бронь.</summary>
    public sealed class ReservedState : IPropertyState
    {
        public static readonly ReservedState Instance = new();

        private static readonly PropertyStatus[] Allowed =
        {
            PropertyStatus.Free,
            PropertyStatus.Sold
        };

        private ReservedState() { }

        public PropertyStatus Status => PropertyStatus.Reserved;

        public string Title => "Забронирован";

        public IReadOnlyCollection<PropertyStatus> AllowedTransitions => Allowed;

        public bool CanTransitionTo(PropertyStatus next) => Allowed.Contains(next);
    }
}