using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DataApi.Models
{
    public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole, string> //DbContext 
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //вот это потому, что Firebird чувствителен к регистрам в отличие от SQL Server.
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("ASPNETUSERS");
                entity.Property(e => e.Id)
                    .HasColumnName("ID");
                entity.Property(e => e.UserName)
                    .HasColumnName("USERNAME");
                entity.Property(e => e.NormalizedUserName)
                    .HasColumnName("NORMALIZEDUSERNAME");
                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL");
                entity.Property(e => e.NormalizedEmail)
                    .HasColumnName("NORMALIZEDEMAIL");
                entity.Property(e => e.EmailConfirmed)
                    .HasColumnName("EMAILCONFIRMED");
                entity.Property(e => e.PasswordHash)
                    .HasColumnName("PASSWORDHASH");
                entity.Property(e => e.SecurityStamp)
                    .HasColumnName("SECURITYSTAMP");
                entity.Property(e => e.ConcurrencyStamp)
                    .HasColumnName("CONCURRENCYSTAMP");
                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("PHONENUMBER");
                entity.Property(e => e.PhoneNumberConfirmed)
                    .HasColumnName("PHONENUMBERCONFIRMED");
                entity.Property(e => e.TwoFactorEnabled)
                    .HasColumnName("TWOFACTORENABLED");
                entity.Property(e => e.LockoutEnd)
                    .HasColumnName("LOCKOUTEND");
                entity.Property(e => e.LockoutEnabled)
                    .HasColumnName("LOCKOUTENABLED");
                entity.Property(e => e.AccessFailedCount)
                    .HasColumnName("ACCESSFAILEDCOUNT");

                // мои поля

                entity.Property(e => e.FullName)
                    .HasColumnName("FULLNAME");
                entity.Property(e => e.KycStatus)
                    .HasColumnName("KYCSTATUS");
                entity.Property(e => e.PassportFilePath)
                    .HasColumnName("PASSPORTFILEPATH");
            });

//  SELECT "a"."ID", "a"."ACCESSFAILEDCOUNT", "a"."CONCURRENCYSTAMP", "a"."EMAIL", "a"."EMAILCONFIRMED",
//   "a"."FULLNAME", "a"."KYCSTATUS", "a"."LockoutEnabled", "a"."LockoutEnd", "a"."NORMALIZEDEMAIL", "a"."NORMALIZEDUSERNAME", 
//   "a"."PASSPORTFILEPATH", "a"."PASSWORDHASH", "a"."PHONENUMBER", "a"."PhoneNumberConfirmed", 
//   "a"."SECURITYSTAMP", "a"."TwoFactorEnabled", "a"."USERNAME"

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("ASPNETROLES");
                entity.Property(e => e.Id)
                    .HasColumnName("ID");
                entity.Property(e => e.Name)
                    .HasColumnName("NAME");
                entity.Property(e => e.NormalizedName)
                    .HasColumnName("NORMALIZEDNAME");
                entity.Property(e => e.ConcurrencyStamp)
                    .HasColumnName("CONCURRENCYSTAMP");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("ASPNETUSERROLES");
                entity.Property(e => e.UserId)
                    .HasColumnName("USERID");
                entity.Property(e => e.RoleId)
                    .HasColumnName("ROLEID");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("ASPNETUSERCLAIMS");
                entity.Property(e => e.Id)
                    .HasColumnName("ID");
                entity.Property(e => e.UserId)
                    .HasColumnName("USERID");
                entity.Property(e => e.ClaimType)
                    .HasColumnName("CLAIMTYPE");
                entity.Property(e => e.ClaimValue)
                    .HasColumnName("CLAIMVALUE");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("ASPNETUSERLOGINS");
                entity.Property(e => e.LoginProvider)
                    .HasColumnName("LOGINPROVIDER");
                entity.Property(e => e.ProviderKey)
                    .HasColumnName("PROVIDERKEY");
                entity.Property(e => e.ProviderDisplayName)
                    .HasColumnName("PROVIDERDISPLAYNAME");
                entity.Property(e => e.UserId)
                    .HasColumnName("USERID");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("ASPNETROLECLAIMS");
                entity.Property(e => e.Id)
                    .HasColumnName("ID");
                entity.Property(e => e.RoleId)
                    .HasColumnName("ROLEID");
                entity.Property(e => e.ClaimType)
                    .HasColumnName("CLAIMTYPE");
                entity.Property(e => e.ClaimValue)
                    .HasColumnName("CLAIMVALUE");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("ASPNETUSERTOKENS");
                entity.Property(e => e.UserId)
                    .HasColumnName("USERID");
                entity.Property(e => e.LoginProvider)
                    .HasColumnName("LOGINPROVIDER");
                entity.Property(e => e.Name)
                    .HasColumnName("NAME");
                entity.Property(e => e.Value)
                    .HasColumnName("TOKEN_VALUE");
            });
        }

        public DbSet<DataItem> DataItems { get; set; }
        public DbSet<Employee> Employees { get; set; }

        //Код для других проектов - не для теко
        //Курьерская программа
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Booking> Bookings { get; set;}
    }
}