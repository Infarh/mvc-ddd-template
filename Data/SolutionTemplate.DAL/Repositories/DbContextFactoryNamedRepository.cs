using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.DAL.Repositories;

public class DbContextFactoryNamedRepository<T> : DbContextFactoryRepository<T>, INamedRepository<T> where T : class, INamedEntity, new()
{
    public DbContextFactoryNamedRepository(IDbContextFactory<SolutionTemplateDB> ContextFactory, ILogger<DbContextFactoryNamedRepository<T>> Logger) : base(ContextFactory, Logger) { }

    public async Task<bool> ExistName(string Name, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).AnyAsync(item => item.Name == Name, Cancel).ConfigureAwait(false);
    }

    public async Task<T> GetByName(string Name, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).FirstOrDefaultAsync(item => item.Name == Name, Cancel).ConfigureAwait(false);
    }

    public async Task<T> DeleteByName(string Name, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        var item = await db.Set<T>()
            //.Select(i => new T { Id = i.Id, Name = i.Name })
           .FirstOrDefaultAsync(i => i.Name == Name, Cancel)
           .ConfigureAwait(false);
        if (item is not null) return await Delete(item, Cancel).ConfigureAwait(false);

        _Logger.LogInformation("При удалении записи с Name: {0} - запись не найдена", Name);
        return null;
    }
}