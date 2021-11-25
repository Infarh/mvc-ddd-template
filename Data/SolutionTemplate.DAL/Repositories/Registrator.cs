using Microsoft.Extensions.DependencyInjection;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.DAL.Repositories;

public static class Registrator
{
    public static IServiceCollection AddSolutionTemplateRepositories(this IServiceCollection services) => services
       .AddScoped(typeof(IRepository<>), typeof(DbRepository<>))
       .AddScoped(typeof(INamedRepository<>), typeof(DbNamedRepository<>))
       .AddScoped(typeof(ITimedRepository<>), typeof(DbNamedRepository<>))
       .AddScoped(typeof(IGPSRepository<>), typeof(DbGPSRepository<>))
    ;

    public static IServiceCollection AddSolutionTemplateRepositoryFactories(this IServiceCollection services) => services
       .AddScoped(typeof(IRepository<>), typeof(DbContextFactoryRepository<>))
       .AddScoped(typeof(IRepository<>), typeof(DbContextFactoryNamedRepository<>))
       .AddScoped(typeof(ITimedRepository<>), typeof(DbContextFactoryNamedRepository<>))
       .AddScoped(typeof(IGPSRepository<>), typeof(DbContextFactoryGPSRepository<>))
    ;
}