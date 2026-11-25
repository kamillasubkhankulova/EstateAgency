using EstateAgency.Data;
using EstateAgency.Models;
using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Enums;

namespace EstateAgency.Services
{
    /// <summary>
    /// Управление сделками
    /// </summary>
    public class DealService
    {
        private readonly AppDbContext _context;

        public DealService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Сделки за период
        /// </summary>
        public async Task<List<Deal>> GetAllDealsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new ArgumentException("Дата начала не может быть позже даты окончания.");

            IQueryable<Deal> query = _context.Deals
                .AsNoTracking()
                .Include(d => d.Client)
                .Include(d => d.Realtor)
                .Include(d => d.DealType)
                .Include(d => d.Property);

            if (startDate.HasValue)
                query = query.Where(d => d.DealDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(d => d.DealDate <= endDate.Value.Date);

            return await query
                .OrderByDescending(d => d.DealDate)
                .ToListAsync();
        }

        /// <summary>
        /// Оформление сделки
        /// </summary>
        public async Task CreateDealAsync(Deal deal)
        {
            ArgumentNullException.ThrowIfNull(deal);

            if (deal.Amount <= 0)
                throw new ArgumentException("Сумма сделки должна быть больше нуля.");

            NormalizeReferences(deal);

            if (deal.ClientId <= 0)
                throw new ArgumentException("Не указан клиент сделки.");

            if (deal.PropertyId <= 0)
                throw new ArgumentException("Не указан объект недвижимости.");

            if (deal.RealtorId <= 0)
                throw new ArgumentException("Не указан риелтор.");

            if (deal.DealTypeId <= 0)
                throw new ArgumentException("Не указан тип сделки.");

            if (!await _context.Clients.AnyAsync(c => c.Id == deal.ClientId))
                throw new ArgumentException("Клиент не найден.");

            if (!await _context.Realtors.AnyAsync(r => r.Id == deal.RealtorId))
                throw new ArgumentException("Риелтор не найден.");

            if (!await _context.DealTypes.AnyAsync(t => t.Id == deal.DealTypeId))
                throw new ArgumentException("Тип сделки не найден.");

            Property property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == deal.PropertyId)
                ?? throw new ArgumentException("Объект недвижимости не найден.");
            if (!property.ChangeStatus(PropertyStatus.Sold))
            {
                throw new InvalidOperationException(
                    $"Нельзя оформить сделку: объект находится в состоянии «{property.State.Title}».");
            }

            Deal entity = new()
            {
                DealDate = deal.DealDate == default ? DateTime.Today : deal.DealDate,
                Amount = deal.Amount,
                ClientId = deal.ClientId,
                PropertyId = deal.PropertyId,
                RealtorId = deal.RealtorId,
                DealTypeId = deal.DealTypeId,
                Status = DealStatus.Completed
            };

            _context.Deals.Add(entity);
            await _context.SaveChangesAsync();
        }
        private static void NormalizeReferences(Deal deal)
        {
            if (deal.Client is not null)
            {
                deal.ClientId = deal.Client.Id;
                deal.Client = null!;
            }

            if (deal.Property is not null)
            {
                deal.PropertyId = deal.Property.Id;
                deal.Property = null!;
            }

            if (deal.Realtor is not null)
            {
                deal.RealtorId = deal.Realtor.Id;
                deal.Realtor = null!;
            }

            if (deal.DealType is not null)
            {
                deal.DealTypeId = deal.DealType.Id;
                deal.DealType = null!;
            }
        }
    }
}
