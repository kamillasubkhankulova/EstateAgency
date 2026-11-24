using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstateAgency.Models;

namespace EstateAgency.Services
{
    /// <summary>
    /// Кто сейчас авторизован в приложении + проверка прав
    /// </summary>
    public static class CurrentUserService
    {
        public static Employee? CurrentUser { get; private set; }

        public static bool IsAuthenticated => CurrentUser is not null;

        public static void SignIn(Employee user) => CurrentUser = user;

        public static void SignOut() => CurrentUser = null;

        public static bool IsAdmin => CurrentUser is Admin;

        public static bool IsRealtor => CurrentUser is Realtor;

        public static bool IsClient => CurrentUser is Client;

        public static bool CanManageProperties => CurrentUser?.CanManageProperties() ?? false;

        public static bool CanViewAllDeals => CurrentUser?.CanViewAllDeals() ?? false;

        public static bool CanDeleteRecords => CurrentUser?.CanDeleteRecords() ?? false;
    }
}