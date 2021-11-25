using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.DAL.Repositories;

/// <summary>Репозиторий сущностей, работающий с контекстом БД</summary>
/// <typeparam name="T">Тип контролируемых сущностей</typeparam>
public class DbRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private readonly SolutionTemplateDB _db;
    protected readonly ILogger<DbRepository<T>> _Logger;

    /// <summary>Источник данных сущностей в контексте БД</summary>
    protected DbSet<T> Set { get; }

    /// <summary>Запрос сущностей из контекста БД</summary>
    protected virtual IQueryable<T> Items => Set;

    /// <summary>Автоматически сохранять вносимые в репозиторий изменения</summary>
    public bool AutoSaveChanges { get; set; } = true;

    public DbRepository(SolutionTemplateDB db, ILogger<DbRepository<T>> Logger)
    {
        _db = db;
        Set = db.Set<T>();
        _Logger = Logger;
    }

    public Task<bool> IsEmpty(CancellationToken Cancel = default) => Set.AnyAsync(Cancel);

    public async Task<bool> ExistId(int Id, CancellationToken Cancel = default) =>
        await Set.AnyAsync(item => item.Id == Id, Cancel).ConfigureAwait(false);

    public async Task<bool> Exist(T item, CancellationToken Cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return await Set.AnyAsync(i => i.Id == item.Id, Cancel).ConfigureAwait(false);
    }

    public async Task<int> GetCount(CancellationToken Cancel = default) => await Items.CountAsync(Cancel).ConfigureAwait(false);

    public async Task<IEnumerable<T>> GetAll(CancellationToken Cancel = default) => await Items.ToArrayAsync(Cancel).ConfigureAwait(false);

    public async Task<IEnumerable<T>> Get(int Skip, int Count, CancellationToken Cancel = default)
    {
        if (Count <= 0) return Enumerable.Empty<T>();

        var query = Items;
        if (Skip > 0) query = query.Skip(Skip);

        return await query.Take(Count).ToArrayAsync(Cancel);
    }

    public async Task<IPage<T>> GetPage(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        if (PageSize <= 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        var query = Items;
        var total_count = await query.CountAsync(Cancel).ConfigureAwait(false);
        if (total_count == 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        if (PageNumber > 0) query = query.Skip(PageNumber * PageSize);
        query = query.Take(PageSize);
        var items = await query.ToArrayAsync(Cancel).ConfigureAwait(false);

        return new Page<T>(items, total_count, PageNumber, PageSize);
    }

    public async Task<T> GetById(int Id, CancellationToken Cancel = default) => Items switch
    {
        DbSet<T> set => await set.FindAsync(new object[] { Id }, Cancel).ConfigureAwait(false),
        { } items => await items.FirstOrDefaultAsync(item => item.Id == Id, Cancel).ConfigureAwait(false),
    };

    public async Task<T> Add(T item, CancellationToken Cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _Logger.LogInformation("Добавление {0} в репозиторий...", item);

        _db.Entry(item).State = EntityState.Added;
        if (AutoSaveChanges) await SaveChangesAsync(Cancel).ConfigureAwait(false);

        _Logger.LogInformation("Добавление {0} в репозиторий выполнено с id: {1}", item, item.Id);

        return item;
    }

    public async Task AddRange(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));
        _Logger.LogInformation("Добавление множества записей в репозиторий...");
        await _db.AddRangeAsync(items, Cancel).ConfigureAwait(false);
        if (AutoSaveChanges) await SaveChangesAsync(Cancel).ConfigureAwait(false);
        _Logger.LogInformation("Добавление множества записей в репозиторий выполнено");
    }

    public async Task<T> Update(T item, CancellationToken Cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _Logger.LogInformation("Обновление id: {0} - {1}...", item.Id, item);


        _db.Entry(item).State = EntityState.Modified;
        if (AutoSaveChanges) await SaveChangesAsync(Cancel).ConfigureAwait(false);

        _Logger.LogInformation("Обновление id: {0} - {1} выполнено", item.Id, item);
        return item;
    }

    public async Task<T> UpdateAsync(int id, Action<T> ItemUpdated, CancellationToken Cancel = default)
    {
        if (await GetById(id, Cancel).ConfigureAwait(false) is not { } item)
            return default;
        ItemUpdated(item);
        if (AutoSaveChanges) await SaveChangesAsync(Cancel).ConfigureAwait(false);
        return item;
    }

    public async Task UpdateRange(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        _db.UpdateRange(items);
        if (AutoSaveChanges) await SaveChangesAsync(Cancel).ConfigureAwait(false);
    }

    public async Task<T> Delete(T item, CancellationToken Cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _Logger.LogInformation("Удаление id: {0} - {1}...", item.Id, item);

        _db.Entry(item).State = EntityState.Deleted;
        if (AutoSaveChanges) await SaveChangesAsync(Cancel).ConfigureAwait(false);

        _Logger.LogInformation("Удаление id: {0} - {1} выполнено", item.Id, item);
        return item;
    }

    public async Task DeleteRange(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        _db.RemoveRange(items);
        if (AutoSaveChanges) await SaveChangesAsync(Cancel).ConfigureAwait(false);
    }

    public async Task<T> DeleteById(int id, CancellationToken Cancel = default)
    {
        var item = await Set.FindAsync(new object[] { id }, Cancel).ConfigureAwait(false);
        if (item is not null) return await Delete(item, Cancel).ConfigureAwait(false);

        _Logger.LogInformation("При удалении записи с id: {0} - запись не найдена", id);
        return null;
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Сохранение изменений в БД");
        var timer = Stopwatch.StartNew();

        var changes_count = await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);

        timer.Stop();
        _Logger.LogInformation("Сохранение изменений в БД  завершено за {0} c. Изменений {1}", timer.Elapsed.TotalSeconds, changes_count);
        return changes_count;
    }
}