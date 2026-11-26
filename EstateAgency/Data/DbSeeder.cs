using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EstateAgency.Models;
using EstateAgency.Services;

namespace EstateAgency.Data
{
    /// <summary>
    /// Тестовые пользователи для проверки входа
    /// </summary>
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Employees.Any())
                return;

            Role adminRole = context.Roles.First(r => r.Name == RoleNames.Admin);
            Role realtorRole = context.Roles.First(r => r.Name == RoleNames.Realtor);
            Role clientRole = context.Roles.First(r => r.Name == RoleNames.Client);
            RequestType purchase = context.RequestTypes.First(t => t.Name == "Покупка");

            context.Employees.AddRange(
                EntityFactory.CreateAdmin(
                    "Субханкулова Камилла Рамилевна", "+7 900 000-00-01", "admin", "admin123", adminRole),

                EntityFactory.CreateRealtor(
                    "Мукашова Алина Серикбаевна", "+7 900 000-00-02", "realtor", "realtor123", realtorRole),

                EntityFactory.CreateClient(
                    "Кузнецов Владислав Андреевич", "+7 900 000-00-03", "client", "client123", clientRole, purchase));

            context.SaveChanges();
        }
    }
}