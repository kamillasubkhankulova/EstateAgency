using RealEstateAgency.Enums;

namespace EstateAgency.States
{
    /// <summary>Свободен: можно забронировать или сразу продать.</summary>
    public sealed class FreeState : IPropertyState
    {
        public static readonly FreeState Instance = new();

        private static readonly PropertyStatus[] Allowed =
        {
            PropertyStatus.Reserved,
            PropertyStatus.Sold
        };

        private FreeState() { }

        public PropertyStatus Status => PropertyStatus.Free;

        public string Title => "Свободен";

        public IReadOnlyCollection<PropertyStatus> AllowedTransitions => Allowed;

        public bool CanTransitionTo(PropertyStatus next) => Allowed.Contains(next);
    }
}