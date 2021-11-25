using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.DAL.Repositories;

public class DbContextFactoryRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    protected IDbContextFactory<SolutionTemplateDB> ContextFactory { get; }
    protected readonly ILogger<DbContextFactoryRepository<T>> _Logger;

    public DbContextFactoryRepository(IDbContextFactory<SolutionTemplateDB> ContextFactory, ILogger<DbContextFactoryRepository<T>> Logger)
    {
        this.ContextFactory = ContextFactory;
        _Logger = Logger;
    }

    /// <summary>Формирование запроса к источнику данных</summary>
    /// <param name="db">Контекст БД</param>
    /// <returns>Запрос сущностей из контекста БД</returns>
    protected virtual IQueryable<T> GetDbQuery(SolutionTemplateDB db) => db.Set<T>();

    public async Task<bool> IsEmpty(CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await db.Set<T>().AnyAsync(Cancel).ConfigureAwait(false);
    }

    public async Task<bool> ExistId(int Id, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).AnyAsync(item => item.Id == Id, Cancel).ConfigureAwait(false);
    }

    public async Task<bool> Exist(T item, CancellationToken Cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).AnyAsync(i => i.Id == item.Id, Cancel).ConfigureAwait(false);
    }

    public async Task<int> GetCount(CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).CountAsync(Cancel).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetAll(CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).ToArrayAsync(Cancel).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> Get(int Skip, int Count, CancellationToken Cancel = default)
    {
        if (Count <= 0) return Enumerable.Empty<T>();

        await using var db = ContextFactory.CreateDbContext();
        var query = Skip <= 0 ? GetDbQuery(db) : GetDbQuery(db).Skip(Skip);
        return await query.Take(Count).ToArrayAsync(Cancel);
    }

    public async Task<IPage<T>> GetPage(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        if (PageSize <= 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        await using var db = ContextFactory.CreateDbContext();

        IQueryable<T> query = GetDbQuery(db);
        var total_count = await query.CountAsync(Cancel).ConfigureAwait(false);
        if (total_count == 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        if (PageNumber > 0) query = query.Skip(PageNumber * PageSize);
        query = query.Take(PageSize);
        var items = await query.ToArrayAsync(Cancel).ConfigureAwait(false);

        return new Page<T>(items, total_count, PageNumber, PageSize);
    }

    public async Task<T> GetById(int Id, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return GetDbQuery(db) switch
        {
            DbSet<T> set => await set.FindAsync(new object[] { Id }, Cancel).ConfigureAwait(false),
            { } query => query.FirstOrDefault(item => item.Id == Id),
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<T> Add(T item, CancellationToken Cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await using var db = ContextFactory.CreateDbContext();
        _Logger.LogInformation("Добавление {0} в репозиторий...", item);

        db.Entry(item).State = EntityState.Added;
        await db.SaveChangesAsync(Cancel).ConfigureAwait(false);

        _Logger.LogInformation("Добавление {0} в репозиторий выполнено с id: {1}", item, item.Id);

        return item;
    }

    public async Task AddRange(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        await db.AddRangeAsync(items, Cancel).ConfigureAwait(false);
        await db.SaveChangesAsync(Cancel).ConfigureAwait(false);
    }

    public async Task<T> Update(T item, CancellationToken Cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await using var db = ContextFactory.CreateDbContext();

        db.Entry(item).State = EntityState.Modified;
        await db.SaveChangesAsync(Cancel).ConfigureAwait(false);

        _Logger.LogInformation("Обновление id: {0} - {1} выполнено", item.Id, item);
        return item;
    }

    public async Task<T> UpdateAsync(int id, Action<T> ItemUpdated, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();

        if (await GetById(id, Cancel).ConfigureAwait(false) is not { } item)
            return default;
        ItemUpdated(item);
        await db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        return item;
    }

    public async Task UpdateRange(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        db.UpdateRange(items);
        await db.SaveChangesAsync(Cancel).ConfigureAwait(false);
    }

    public async Task<T> Delete(T item, CancellationToken Cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await using var db = ContextFactory.CreateDbContext();

        db.Entry(item).State = EntityState.Deleted;
        await db.SaveChangesAsync(Cancel).ConfigureAwait(false);

        _Logger.LogInformation("Удаление id: {0} - {1} выполнено", item.Id, item);
        return item;
    }

    public async Task DeleteRange(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        db.RemoveRange(items);
        await db.SaveChangesAsync(Cancel).ConfigureAwait(false);
    }

    public async Task<T> DeleteById(int id, CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        var item = await db.Set<T>().FindAsync(new object[] { id }, Cancel).ConfigureAwait(false);
        if (item is not null) return await Delete(item, Cancel).ConfigureAwait(false);

        _Logger.LogInformation("При удалении записи с id: {0} - запись не найдена", id);
        return null;

    }
}