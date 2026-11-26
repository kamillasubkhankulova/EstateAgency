using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstateAgency.Data;
using EstateAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace EstateAgency.Services
{
    /// <summary>
    /// Вход и регистрация пользователей
    /// </summary>
    public class AuthService
    {
        private const int MinPasswordLength = 6;
        private const int MinLoginLength = 3;

        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Проверяет логин и пароль. При успехе запоминает пользователя.
        /// </summary>
        public async Task<(bool Success, string? Error, Employee? User)> LoginAsync(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return (false, "Введите логин и пароль.", null);

            string normalizedLogin = login.Trim();

            Employee? user = await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Login.ToLower() == normalizedLogin.ToLower());

            if (user is null)
                return (false, "Пользователь с таким логином не найден.", null);

            if (!PasswordHasher.Verify(password, user.PasswordHash))
                return (false, "Неверный пароль.", null);

            CurrentUserService.SignIn(user);
            return (true, null, user);
        }

        /// <summary>
        /// Регистрирует пользователя
        /// </summary>
        public (bool Success, string? Error, Employee? User) Register(
            string fullName,
            string phone,
            string login,
            string password,
            int roleId,
            int? requestTypeId = null)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (false, "Укажите ФИО.", null);

            if (string.IsNullOrWhiteSpace(phone))
                return (false, "Укажите телефон.", null);

            if (string.IsNullOrWhiteSpace(login) || login.Trim().Length < MinLoginLength)
                return (false, $"Логин должен быть не короче {MinLoginLength} символов.", null);

            if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
                return (false, $"Пароль должен быть не короче {MinPasswordLength} символов.", null);

            string normalizedLogin = login.Trim();

            if (_context.Employees.Any(e => e.Login.ToLower() == normalizedLogin.ToLower()))
                return (false, "Этот логин уже занят.", null);

            Role? role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
            if (role is null)
                return (false, "Роль не найдена.", null);

            Employee user;

            switch (role.Name)
            {
                case RoleNames.Admin:
                    user = EntityFactory.CreateAdmin(fullName.Trim(), phone.Trim(), normalizedLogin, password, role);
                    break;

                case RoleNames.Realtor:
                    user = EntityFactory.CreateRealtor(fullName.Trim(), phone.Trim(), normalizedLogin, password, role);
                    break;

                case RoleNames.Client:
                    {
                        if (requestTypeId is null)
                            return (false, "Для клиента укажите тип запроса.", null);

                        RequestType? requestType =
                            _context.RequestTypes.FirstOrDefault(t => t.Id == requestTypeId.Value);

                        if (requestType is null)
                            return (false, "Тип запроса не найден.", null);

                        user = EntityFactory.CreateClient(
                            fullName.Trim(), phone.Trim(), normalizedLogin, password, role, requestType);
                        break;
                    }

                default:
                    return (false, "Неизвестная роль.", null);
            }

            try
            {
                _context.Employees.Add(user);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return (false, "Не удалось сохранить пользователя. Проверьте данные.", null);
            }

            return (true, null, user);
        }

        public void Logout() => CurrentUserService.SignOut();

        /// <summary>
        /// Список ролей для выпадающего списка на форме регистрации
        /// </summary>
        public List<Role> GetRoles() =>
            _context.Roles.OrderBy(r => r.Id).ToList();

        /// <summary>
        /// Список типов запроса для выпадающего списка.
        /// </summary>
        public List<RequestType> GetRequestTypes() =>
            _context.RequestTypes.OrderBy(t => t.Id).ToList();
    }
}