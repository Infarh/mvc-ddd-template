using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolutionTemplate.DAL.Context;

namespace SolutionTemplate.DAL;

public static class ServicesExtensions
{
    /// <summary>Получить контекст БД</summary>
    /// <param name="services">Провайдер сервисов</param>
    /// <returns>Контекст БД</returns>
    public static SolutionTemplateDB GetSolutionTemplateDB(this IServiceProvider services) => services.GetRequiredService<SolutionTemplateDB>();

    /// <summary>Получить фабрику контекстов БД</summary>
    /// <param name="services">Провайдер сервисов</param>
    /// <returns>Фабрика контекстов БД</returns>
    public static IDbContextFactory<SolutionTemplateDB> GetSolutionTemplateDBFactory(this IServiceProvider services) =>
        services.GetRequiredService<IDbContextFactory<SolutionTemplateDB>>();

    /// <summary>Получить контекст БД</summary>
    /// <param name="scope">Область видимости провайдера сервисов</param>
    /// <returns>Контекст БД</returns>
    public static SolutionTemplateDB GetSolutionTemplateDB(this IServiceScope scope) => scope.ServiceProvider.GetSolutionTemplateDB();

    /// <summary>Получить фабрику контекстов БД</summary>
    /// <param name="scope">Область видимости провайдера сервисов</param>
    /// <returns>Фабрика контекстов БД</returns>
    public static IDbContextFactory<SolutionTemplateDB> GetSolutionTemplateDBFactory(this IServiceScope scope) => scope.ServiceProvider.GetSolutionTemplateDBFactory();
}