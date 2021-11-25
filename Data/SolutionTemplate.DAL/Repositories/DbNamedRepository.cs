using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.DAL.Repositories;

public class DbNamedRepository<T> : DbRepository<T>, INamedRepository<T> where T : class, INamedEntity, new()
{
    public DbNamedRepository(SolutionTemplateDB db, ILogger<DbNamedRepository<T>> Logger) : base(db, Logger) { }

    public async Task<bool> ExistName(string Name, CancellationToken Cancel = default) =>
        await Set.AnyAsync(item => item.Name == Name, Cancel).ConfigureAwait(false);

    public async Task<T> GetByName(string Name, CancellationToken Cancel = default) =>
        await Items.FirstOrDefaultAsync(item => item.Name == Name, Cancel).ConfigureAwait(false);

    public async Task<T> DeleteByName(string Name, CancellationToken Cancel = default)
    {
        var item = Set.Local.FirstOrDefault(i => i.Name == Name)
            ?? await Set
                //.Select(i => new T { Id = i.Id, Name = i.Name })
               .FirstOrDefaultAsync(i => i.Name == Name, Cancel)
               .ConfigureAwait(false);
        if (item is not null) return await Delete(item, Cancel).ConfigureAwait(false);

        _Logger.LogInformation("При удалении записи с Name: {0} - запись не найдена", Name);
        return null;
    }
}