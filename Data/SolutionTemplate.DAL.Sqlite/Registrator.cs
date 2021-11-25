using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.DAL.Repositories;

namespace SolutionTemplate.DAL.Sqlite;

/// <summary>Регистратор сервисов слоя данных для Sqlite</summary>
public static class Registrator
{
    /// <summary>Добавить контекст данных в контейнер сервисов для подключения к Sqlite</summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="ConnectionString">Строка подключения к серверу</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddSolutionTemplateDbContextSqlite(this IServiceCollection services, string ConnectionString) => 
        services
           .AddDbContext<SolutionTemplateDB>(opt => opt.UseSqlite(ConnectionString, o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)))
           .AddScoped<IDbInitializer, SolutionTemplateDBInitializer>()
           .AddSolutionTemplateRepositories();

    /// <summary>Добавить фабрику контекста данных в контейнер сервисов для подключения к Sqlite</summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="ConnectionString">Строка подключения к серверу</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddSolutionTemplateDbContextFactorySqlite(this IServiceCollection services, string ConnectionString) => 
        services
           .AddDbContextFactory<SolutionTemplateDB>(opt => opt.UseSqlite(ConnectionString, o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)))
           .AddScoped<IDbInitializer, SolutionTemplateDBInitializer>()
           .AddSolutionTemplateRepositoryFactories();
}