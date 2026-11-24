using EstateAgency.Models;
using RealEstateAgency.Enums;
using EstateAgency.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateAgency.Data
{
    /// <summary>Фабрика сущностей (паттерн Factory).</summary>
    public static class EntityFactory
    {
        public static Admin CreateAdmin(string fullName, string phone, string login,
                                        string password, Role role) =>
            new()
            {
                FullName = fullName,
                Phone = phone,
                Login = login,
                PasswordHash = PasswordHasher.Hash(password),
                Role = role
            };

        public static Realtor CreateRealtor(string fullName, string phone, string login,
                                            string password, Role role) =>
            new()
            {
                FullName = fullName,
                Phone = phone,
                Login = login,
                PasswordHash = PasswordHasher.Hash(password),
                Role = role
            };

        public static Client CreateClient(string fullName, string phone, string login,
                                          string password, Role role, RequestType requestType) =>
            new()
            {
                FullName = fullName,
                Phone = phone,
                Login = login,
                PasswordHash = PasswordHasher.Hash(password),
                Role = role,
                RequestType = requestType,
                RegistrationDate = DateTime.Now
            };

        public static Property CreateProperty(Address address, PropertyType type, double area,
                                              int rooms, int floor, decimal price, Realtor realtor) =>
            new()
            {
                Address = address,
                Type = type,
                Area = area,
                Rooms = rooms,
                Floor = floor,
                Price = price,
                Status = PropertyStatus.Free,
                Realtor = realtor
            };

        public static Deal CreateDeal(Client client, Property property, Realtor realtor,
                                      DealType dealType, decimal amount) =>
            new()
            {
                Client = client,
                Property = property,
                Realtor = realtor,
                DealType = dealType,
                Amount = amount,
                DealDate = DateTime.Today,
                Status = DealStatus.New
            };

        public static Address CreateAddress(string city, string street, string house, string apartment = "") =>
            new()
            {
                City = city,
                Street = street,
                House = house,
                Apartment = apartment
            };
    }
}