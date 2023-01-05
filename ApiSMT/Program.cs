using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using Microsoft.AspNetCore.Connections;

namespace ApiSMT
{
    /// <summary>
    /// Classe Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Função main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var configuracao = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuracao).CreateLogger();

            try
            {
                Log.Information("A Api foi iniciada com sucesso!!!");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Erro ao iniciar sistema");
            }
        }

        /// <summary>
        /// Função CreateHostBuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseContentRoot(Directory.GetCurrentDirectory()).UseIISIntegration().UseKestrel().ConfigureKestrel(options =>
            {
                options.ListenLocalhost(777);
                options.ListenAnyIP(777);

            }).UseStartup<Startup>();
        }).UseWindowsService();
    }
}
