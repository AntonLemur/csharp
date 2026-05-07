using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using DataApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;

namespace dataws
{
    public class Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // Ваша регистрация сервисов (AddDbContext и т.д.)
            services.AddDbContext<DataContext>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    //для production это всё убрать 
                    options.Password.RequiredLength = 4;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();
            services.AddMvc(options => options.EnableEndpointRouting = false); 
            services.AddScoped<IDataRepository, DataRepository>();           
        }

        // Здесь тоже меняем IHostingEnvironment на IWebHostEnvironment
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Если вы не используете статические файлы (HTML, CSS внутри папки проекта), просто удалите или закомментируйте эту строку            
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication();   // 👈 обязательно
            app.UseAuthorization();    // 👈 обязательно
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Добавляем для обычных страниц (Index, Customers и т.д.)
                endpoints.MapControllerRoute(
                    name: "default",
                    // pattern: "{controller=Data}/{action=Index}/{id?}"); //для api
                    pattern: "{controller=Home}/{action=Index}/{id?}"); //для веб-приложения               
            });

            // Если вы используете app.UseRouting() и app.UseEndpoints(), 
            // то старый метод UseMvc() больше не нужен (и может вызывать конфликты в .NET Core 3.0+).
            // app.UseMvc();
        }
    }
}