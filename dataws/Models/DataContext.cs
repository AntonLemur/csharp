using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace DataApi.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var Configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
            
            optionsBuilder.UseFirebird(Configuration["ConnectionString"]);
        }
        

        public DbSet<DataItem> DataItems { get; set; }
        public DbSet<Employee> Employees { get; set; }

        //Код для других проектов - не для теко
        //Курьерская программа
        public DbSet<OrderCourier> OrdersCourier { get; set; }

    }
}