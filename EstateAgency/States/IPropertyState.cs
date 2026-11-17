using RealEstateAgency.Enums;

namespace EstateAgency.States
{
    /// <summary>Состояние объекта недвижимости (паттерн State).</summary>
    public interface IPropertyState
    {
        PropertyStatus Status { get; }

        /// <summary>Название состояния для интерфейса.</summary>
        string Title { get; }

        /// <summary>Допустимые переходы из текущего состояния.</summary>
        IReadOnlyCollection<PropertyStatus> AllowedTransitions { get; }

        bool CanTransitionTo(PropertyStatus next);
    }
}