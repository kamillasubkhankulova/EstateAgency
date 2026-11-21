using EstateAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace EstateAgency.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Realtor> Realtors => Set<Realtor>();
        public DbSet<Client> Clients => Set<Client>();

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<DealType> DealTypes => Set<DealType>();
        public DbSet<RequestType> RequestTypes => Set<RequestType>();

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Deal> Deals => Set<Deal>();

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(App.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- Справочники ----------
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasIndex(r => r.Name).IsUnique();
                entity.HasData(
                    new Role { Id = 1, Name = "Администратор" },
                    new Role { Id = 2, Name = "Риелтор" },
                    new Role { Id = 3, Name = "Клиент" });
            });

            modelBuilder.Entity<DealType>(entity =>
            {
                entity.ToTable("Deal_Types");
                entity.HasIndex(t => t.Name).IsUnique();
                entity.HasData(
                    new DealType { Id = 1, Name = "Продажа" },
                    new DealType { Id = 2, Name = "Аренда" });
            });

            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.ToTable("Request_Types");
                entity.HasIndex(t => t.Name).IsUnique();
                entity.HasData(
                    new RequestType { Id = 1, Name = "Покупка" },
                    new RequestType { Id = 2, Name = "Аренда" });
            });

            // ---------- Сотрудники: TPH ----------
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasDiscriminator<string>("Discriminator")
                      .HasValue<Admin>("Admin")
                      .HasValue<Realtor>("Realtor")
                      .HasValue<Client>("Client");

                entity.HasIndex(e => e.Login).IsUnique();

                // Связь с ролью — от базового класса
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Employees)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Client>()
                .HasOne(c => c.RequestType)
                .WithMany(t => t.Clients)
                .HasForeignKey(c => c.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Properties");

                entity.OwnsOne(p => p.Address, address =>
                {
                    address.Property(a => a.City).HasMaxLength(100).IsRequired();
                    address.Property(a => a.Street).HasMaxLength(150).IsRequired();
                    address.Property(a => a.House).HasMaxLength(20).IsRequired();
                    address.Property(a => a.Apartment).HasMaxLength(20);
                });

                entity.Property(p => p.Price).HasPrecision(18, 2);

                entity.HasOne(p => p.Realtor)
                      .WithMany(r => r.Properties)
                      .HasForeignKey(p => p.RealtorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Deal>(entity =>
            {
                entity.ToTable("Deals");

                entity.Property(d => d.Amount).HasPrecision(18, 2);

                entity.HasOne(d => d.DealType)
                      .WithMany(t => t.Deals)
                      .HasForeignKey(d => d.DealTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Client)
                      .WithMany(c => c.Deals)
                      .HasForeignKey(d => d.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Property)
                      .WithMany(p => p.Deals)
                      .HasForeignKey(d => d.PropertyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Realtor)
                      .WithMany(r => r.Deals)
                      .HasForeignKey(d => d.RealtorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}