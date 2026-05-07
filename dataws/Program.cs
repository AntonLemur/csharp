using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace dataws
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Приложение запускается...");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                 Console.WriteLine("Критическая ошибка при старте:");
                 Console.WriteLine(ex.ToString());
            }
        }

        public static IHost BuildWebHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                // Вместо создания нового ConfigurationBuilder, 
                    // используем тот, который хост уже создал «под капотом»
                    // webBuilder.ConfigureAppConfiguration((hostingContext, config) => {
                    //     // Здесь можно ничего не писать, всё уже загружено
                    // });

                    // Правильный способ получить URL из уже загруженного конфига:
                    // webBuilder.UseKestrel((context, options) => {
                    //     var url = context.Configuration["Url"];
                    //     // Настройка порта через Kestrel или просто через UseUrls выше
                    // });
                    
                    // Или еще проще, если вам нужно просто UseUrls:
                    // webBuilder.UseUrls( ... );

            // Используем уже загруженную конфигурацию
                webBuilder.ConfigureKestrel((context, options) =>
                {
                    // Читаем значение из вашего appsettings.json
                    var url = context.Configuration["Url"]; 

                    if (!string.IsNullOrEmpty(url))
                    {
                        // Если в Url записано что-то вроде "http://localhost:5030"
                        webBuilder.UseUrls(url);
                    }
                });                    
            })
            .Build();
        } 
    }
}
