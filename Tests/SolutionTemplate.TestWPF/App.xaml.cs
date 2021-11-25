using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolutionTemplate.DAL.Sqlite;
using SolutionTemplate.DAL.SqlServer;

namespace SolutionTemplate.TestWPF;

public partial class App
{
    static App() => ConfigureServices += OnConfigureServices;

    private static void OnConfigureServices(HostBuilderContext host, IServiceCollection services)
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
}