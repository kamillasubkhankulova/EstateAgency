using System;
using EstateAgency.Data;
using EstateAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace EstateAgency.Services
{
    /// <summary>
    /// Управление клиентами агентства
    /// </summary>
    public class ClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _context.Clients
                .AsNoTracking()
                .Include(c => c.RequestType)
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task AddClientAsync(Client client)
        {
            ArgumentNullException.ThrowIfNull(client);

            if (string.IsNullOrWhiteSpace(client.FullName))
                throw new ArgumentException("Укажите ФИО клиента.");

            if (string.IsNullOrWhiteSpace(client.Phone))
                throw new ArgumentException("Укажите телефон клиента.");

            if (string.IsNullOrWhiteSpace(client.Login))
                throw new ArgumentException("Укажите логин клиента.");

            if (string.IsNullOrWhiteSpace(client.PasswordHash))
                throw new ArgumentException("Не задан пароль клиента.");

            string login = client.Login.Trim();

            if (await _context.Employees.AnyAsync(e => e.Login.ToLower() == login.ToLower()))
                throw new ArgumentException("Этот логин уже занят.");

            client.Login = login;

            if (client.RequestType is not null)
            {
                client.RequestTypeId = client.RequestType.Id;
                client.RequestType = null;
            }

            if (client.RequestTypeId is null or 0)
                throw new ArgumentException("Укажите тип запроса клиента.");

            if (!await _context.RequestTypes.AnyAsync(t => t.Id == client.RequestTypeId))
                throw new ArgumentException("Тип запроса не найден.");
            if (client.Role is not null)
            {
                client.RoleId = client.Role.Id;
                client.Role = null!;
            }

            if (client.RoleId <= 0)
            {
                Role clientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.Client)
                    ?? throw new InvalidOperationException("Роль «Клиент» не найдена в справочнике.");

                client.RoleId = clientRole.Id;
            }

            if (client.RegistrationDate == default)
                client.RegistrationDate = DateTime.Now;

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Редактирование карточки клиента
        /// </summary>
        public async Task UpdateClientAsync(Client client)
        {
            ArgumentNullException.ThrowIfNull(client);

            if (string.IsNullOrWhiteSpace(client.FullName))
                throw new ArgumentException("Укажите ФИО клиента.");

            if (string.IsNullOrWhiteSpace(client.Phone))
                throw new ArgumentException("Укажите телефон клиента.");

            Client existing = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == client.Id)
                ?? throw new KeyNotFoundException($"Клиент с Id={client.Id} не найден.");

            if (client.RequestType is not null)
                client.RequestTypeId = client.RequestType.Id;

            existing.FullName = client.FullName.Trim();
            existing.Phone = client.Phone.Trim();
            existing.RequestTypeId = client.RequestTypeId;

            await _context.SaveChangesAsync();
        }
        public async Task DeleteClientAsync(int id)
        {
            Client client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException($"Клиент с Id={id} не найден.");

            if (await _context.Deals.AnyAsync(d => d.ClientId == id))
                throw new InvalidOperationException("Нельзя удалить клиента: по нему есть сделки.");

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }
}
