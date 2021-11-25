using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Extensions;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.DAL.Repositories;

/// <summary>Репозиторий географических данных</summary>
/// <typeparam name="T">Тип сущности репозитория</typeparam>
public class DbGPSRepository<T> : DbRepository<T>, IGPSRepository<T> where T : class, IGPSEntity, new()
{
    /// <summary>Инициализация нового экземпляра <see cref="DbGPSRepository{T}"/></summary>
    /// <param name="db">Контекст БД</param>
    /// <param name="Logger">Логгер</param>
    public DbGPSRepository(SolutionTemplateDB db, ILogger<DbGPSRepository<T>> Logger) : base(db, Logger) { }

    /// <summary>Существует ли в репозитории сущности в заданном радиусе</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="RangeInMeters">Радиус поиска в метрах</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Истина, если в заданном радиусе поиска есть сущности</returns>
    public Task<bool> ExistInLocation(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default) =>
        Set
           .OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .AnyAsync(Cancel);

    /// <summary>Определение числа сущностей, попадающий в заданный радиус поиска</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="RangeInMeters">Радиус поиска в метрах</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Число сущностей, попадающих в заданный радиус поиска</returns>
    public Task<int> GetCountInLocation(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default) =>
        Set
           .OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .CountAsync(Cancel);

    /// <summary>Получить все сущности из заданного радиуса поиска</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="RangeInMeters">Радиус поиска в метрах</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Перечисление сущностей, попадающих в заданный радиус поиска</returns>
    public async Task<IEnumerable<T>> GetAllByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default) =>
        await Items
           .OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .ToArrayAsync(Cancel)
           .ConfigureAwait(false);

    /// <summary>Получить все сущности из заданного радиуса поиска</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="RangeInMeters">Радиус поиска в метрах</param>
    /// <param name="Skip">Число пропускаемых сущностей в начале выборки</param>
    /// <param name="Take">Число извлекаемых сущностей из результатов запроса</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Перечисление сущностей, попадающих в заданный радиус поиска</returns>
    public async Task<IEnumerable<T>> GetAllByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        int Skip,
        int Take,
        CancellationToken Cancel = default) =>
        await Items
           .OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .Skip(Skip)
           .Take(Take)
           .ToArrayAsync(Cancel)
           .ConfigureAwait(false);

    /// <summary>Получить сущность, ближайшую к указанной точке</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Сущность, ближайшая к указанной точке</returns>
    public Task<T> GetByLocation(
        double Latitude,
        double Longitude,
        CancellationToken Cancel = default) =>
        Items
           .OrderByDistance(Latitude, Longitude)
           .FirstAsync(Cancel);

    /// <summary>Получить сущность, ближайшую к указанной точке с ограничением радиуса поиска</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="RangeInMeters">Радиус поиска в метрах</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Сущность, ближайшая к указанной точке, попадающая в заданный радиус поиска</returns>
    public Task<T> GetByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default) =>
        Items.OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .FirstOrDefaultAsync(Cancel);

    /// <summary>Получить страницу с записями с указанном радиусе поиска</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="RangeInMeters">Радиус поиска в метрах</param>
    /// <param name="PageNumber">Номер страницы (начиная с нуля)</param>
    /// <param name="PageSize">Размер страницы</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Страница с записями в указанном радиусе поиска</returns>
    public async Task<IPage<T>> GetPageByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        int PageNumber,
        int PageSize,
        CancellationToken Cancel = default)
    {
        if (PageSize <= 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        var query = Items.OrderByDistanceInRange(Latitude, Longitude, RangeInMeters);
        var total_count = await query.CountAsync(Cancel).ConfigureAwait(false);
        if (total_count == 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        if (PageNumber > 0) query = query.Skip(PageNumber * PageSize);
        query = query.Take(PageSize);
        var items = await query.ToArrayAsync(Cancel).ConfigureAwait(false);

        return new Page<T>(items, total_count, PageNumber, PageSize);
    }

    /// <summary>Удалить сущность, ближайшую к заданной точке</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Удалённая сущность</returns>
    public async Task<T> DeleteByLocation(double Latitude, double Longitude, CancellationToken Cancel = default) =>
        await GetByLocation(Latitude, Longitude, Cancel).ConfigureAwait(false) is { } item
            ? await Delete(item, Cancel)
            : default;

    /// <summary>Удалить сущность, ближайшую к центру заданного радиуса поиска</summary>
    /// <param name="Latitude">Широта</param>
    /// <param name="Longitude">Долгота</param>
    /// <param name="RangeInMeters">Радиус поиска в метрах</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Удалённая сущность</returns>
    public async Task<T> DeleteByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default) =>
        await GetByLocationInRange(Latitude, Longitude, RangeInMeters, Cancel).ConfigureAwait(false) is { } item
            ? await Delete(item, Cancel)
            : default;
}