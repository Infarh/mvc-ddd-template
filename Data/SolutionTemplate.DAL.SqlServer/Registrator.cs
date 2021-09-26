using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.DAL.Repositories;

namespace SolutionTemplate.DAL.SqlServer
{
    /// <summary>Регистратор сервисов слоя данных для SQL-сервера</summary>
    public static class Registrator
    {
        /// <summary>Добавить контекст данных в контейнер сервисов для подключения к SQL-серверу</summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="ConnectionString">Строка подключения к серверу</param>
        /// <returns>Коллекция сервисов</returns>
        public static IServiceCollection AddSolutionTemplateDbContextSqlServer(this IServiceCollection services, string ConnectionString) =>
            services
               .AddDbContext<SolutionTemplateDB>(opt => opt.UseSqlServer(ConnectionString, o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)))
               .AddScoped<IDbInitializer, SolutionTemplateDBInitializer>()
               .AddSolutionTemplateRepositories();

        /// <summary>Добавить фабрику контекста данных в контейнер сервисов для подключения к SQL-серверу</summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="ConnectionString">Строка подключения к серверу</param>
        /// <returns>Коллекция сервисов</returns>
        public static IServiceCollection AddSolutionTemplateDbContextFactorySqlServer(this IServiceCollection services, string ConnectionString) =>
            services
               .AddDbContextFactory<SolutionTemplateDB>(opt => opt.UseSqlServer(ConnectionString, o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)))
               .AddScoped<IDbInitializer, SolutionTemplateDBInitializer>()
               .AddSolutionTemplateRepositoryFactories();
    }
}
