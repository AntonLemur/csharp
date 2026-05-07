using Microsoft.EntityFrameworkCore;
using testdb.Models;

namespace testdb.Data;

public class AppDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<SpecItem> SpecItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Настройка подключения для Firebird
        optionsBuilder.UseFirebird("User=SYSDBA;Password=56911317;Database=teko;DataSource=localhost;Port=3050;Dialect=3;Charset=UTF8;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SpecItem>(entity =>
        {
            entity.ToTable("SPECIFICATIONS");
            entity.HasKey(e => e.Id);

            // Настройка связи "Один родитель - много детей"
            entity.HasOne(d => d.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Запрет на удаление при наличии детей
        });
    }    
}