using RealEstateAgency.Enums;

namespace EstateAgency.States
{
    /// <summary>Получение объекта состояния по значению enum из базы.</summary>
    public static class PropertyStates
    {
        public static IPropertyState Resolve(PropertyStatus status) => status switch
        {
            PropertyStatus.Free => FreeState.Instance,
            PropertyStatus.Reserved => ReservedState.Instance,
            PropertyStatus.Sold => SoldState.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Неизвестный статус объекта")
        };
    }
}