using WebCityEvents.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;

namespace WebCityEvents.Data
{
    public class EventContext : DbContext
    {
        public EventContext(DbContextOptions<EventContext> options) : base(options) { }

        public DbSet<Place> Places { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TicketOrder> TicketOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) // Не вызываем конфигурацию, если контекст замокирован
            {
                ConfigurationBuilder builder = new();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();

                string connectionString = configuration.GetConnectionString("LocalSQLConnection");

                optionsBuilder
                    .UseSqlServer(connectionString)
                    .LogTo(message => System.Diagnostics.Debug.WriteLine(message));
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketOrder>()
                .HasKey(t => t.OrderID);
        }
    }
}