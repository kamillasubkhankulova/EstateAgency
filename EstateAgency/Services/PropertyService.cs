using EstateAgency.Data;
using EstateAgency.Models;
using EstateAgency.States;
using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Enums;

namespace EstateAgency.Services
{
    /// <summary>
    /// Управление объектами недвижимости
    /// </summary>
    public class PropertyService
    {
        private readonly AppDbContext _context;

        public PropertyService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Список объектов с фильтрацией по цене, комнатам и типу
        /// </summary>
        public async Task<List<Property>> GetAllPropertiesAsync(
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? rooms = null,
            PropertyType? type = null)
        {
            if (minPrice is < 0 || maxPrice is < 0)
                throw new ArgumentException("Цена не может быть отрицательной.");

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                throw new ArgumentException("Минимальная цена не может быть больше максимальной.");

            IQueryable<Property> query = _context.Properties
                .AsNoTracking()
                .Include(p => p.Realtor);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (rooms.HasValue)
                query = query.Where(p => p.Rooms == rooms.Value);

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);

            return await query
                .OrderBy(p => p.Price)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Некорректный идентификатор объекта.", nameof(id));

            return await _context.Properties
                .AsNoTracking()
                .Include(p => p.Realtor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPropertyAsync(Property property)
        {
            ArgumentNullException.ThrowIfNull(property);

            ValidateProperty(property);
            if (property.Realtor is not null)
            {
                property.RealtorId = property.Realtor.Id;
                property.Realtor = null!;
            }
            if (property.RealtorId <= 0)
                throw new ArgumentException("Не указан риелтор объекта.");

            if (!await _context.Realtors.AnyAsync(r => r.Id == property.RealtorId))
                throw new ArgumentException("Риелтор не найден.");
            property.Status = PropertyStatus.Free;

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Обновление объекта
        /// </summary>
        public async Task UpdatePropertyAsync(Property property)
        {
            ArgumentNullException.ThrowIfNull(property);

            ValidateProperty(property);

            Property existing = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == property.Id)
                ?? throw new KeyNotFoundException($"Объект с Id={property.Id} не найден.");

            if (existing.Status != property.Status && !existing.ChangeStatus(property.Status))
            {
                throw new InvalidOperationException(
                    $"Недопустимый переход статуса: «{existing.State.Title}» → «{PropertyStates.Resolve(property.Status).Title}».");
            }

            if (property.Realtor is not null)
                property.RealtorId = property.Realtor.Id;

            if (property.RealtorId <= 0)
                throw new ArgumentException("Не указан риелтор объекта.");

            existing.Address.City = property.Address.City;
            existing.Address.Street = property.Address.Street;
            existing.Address.House = property.Address.House;
            existing.Address.Apartment = property.Address.Apartment;

            existing.Type = property.Type;
            existing.Area = property.Area;
            existing.Rooms = property.Rooms;
            existing.Floor = property.Floor;
            existing.Price = property.Price;
            existing.RealtorId = property.RealtorId;

            await _context.SaveChangesAsync();
        }
        public async Task DeletePropertyAsync(int id)
        {
            Property property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException($"Объект с Id={id} не найден.");

            if (property.Status == PropertyStatus.Sold)
                throw new InvalidOperationException("Нельзя удалить проданный объект — он участвует в сделке.");

            if (await _context.Deals.AnyAsync(d => d.PropertyId == id))
                throw new InvalidOperationException("Нельзя удалить объект: по нему уже есть сделки.");

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
        }

        private static void ValidateProperty(Property property)
        {
            if (property.Address is null ||
                string.IsNullOrWhiteSpace(property.Address.City) ||
                string.IsNullOrWhiteSpace(property.Address.Street) ||
                string.IsNullOrWhiteSpace(property.Address.House))
            {
                throw new ArgumentException("Адрес заполнен не полностью.");
            }

            if (property.Price <= 0)
                throw new ArgumentException("Цена должна быть больше нуля.");

            if (property.Area <= 0)
                throw new ArgumentException("Площадь должна быть больше нуля.");

            if (property.Rooms <= 0)
                throw new ArgumentException("Количество комнат должно быть больше нуля.");

            if (property.Floor < 0)
                throw new ArgumentException("Этаж не может быть отрицательным.");
        }
    }
}
