using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolutionTemplate.DAL.Sqlite;
using SolutionTemplate.DAL.SqlServer;

namespace SolutionTemplate.TestConsole;

internal class Program
{
    private static IHost __Hosting;

    public static IHost Hosting => __Hosting ??= CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

    public static IHostBuilder CreateHostBuilder(string[] args) => Host
       .CreateDefaultBuilder(args)
       .ConfigureServices(ConfigureServices);

    private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
    {
        var db_type = host.Configuration["Database"];
        switch (db_type)
        {
            default: throw new NotSupportedException($"Тип БД {db_type} не поддерживается");

            case "SqlServer":
                services.AddSolutionTemplateDbContextFactorySqlServer(host.Configuration.GetConnectionString(db_type));
                break;

            case "Sqlite":
                services.AddSolutionTemplateDbContextFactorySqlite(host.Configuration.GetConnectionString(db_type));
                break;
        }
    }

    public static async Task Main(string[] args)
    {
        using var host = Hosting;
        await host.StartAsync();

        Console.WriteLine("Hello World!");

        await host.StopAsync();

        Console.WriteLine("Completed.");
    }
}