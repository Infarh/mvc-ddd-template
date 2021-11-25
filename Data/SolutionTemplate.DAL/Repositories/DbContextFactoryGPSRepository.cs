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

public class DbContextFactoryGPSRepository<T> : DbContextFactoryRepository<T>, IGPSRepository<T> where T : class, IGPSEntity, new()
{
    public DbContextFactoryGPSRepository(IDbContextFactory<SolutionTemplateDB> db, ILogger<DbContextFactoryGPSRepository<T>> Logger) : base(db, Logger) { }

    public async Task<bool> ExistInLocation(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await db.Set<T>()
           .OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .AnyAsync(Cancel)
           .ConfigureAwait(false);
    }

    public async Task<int> GetCountInLocation(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await db.Set<T>()
           .OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .CountAsync(Cancel)
           .ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetAllByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .ToArrayAsync(Cancel)
           .ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetAllByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        int Skip,
        int Take,
        CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db)
           .OrderByDistanceInRange(Latitude, Longitude, RangeInMeters)
           .Skip(Skip)
           .Take(Take)
           .ToArrayAsync(Cancel)
           .ConfigureAwait(false);
    }

    public async Task<T> GetByLocation(
        double Latitude,
        double Longitude,
        CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db)
           .OrderByDistance(Latitude, Longitude)
           .FirstAsync(Cancel)
           .ConfigureAwait(false);
    }

    public async Task<T> GetByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db)
           .OrderByDistance(Latitude, Longitude)
           .FirstOrDefaultAsync(Cancel)
           .ConfigureAwait(false);
    }

    public async Task<IPage<T>> GetPageByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        int PageNumber,
        int PageSize,
        CancellationToken Cancel = default)
    {
        if (PageSize <= 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        await using var db = ContextFactory.CreateDbContext();
        var query = GetDbQuery(db).OrderByDistanceInRange(Latitude, Longitude, RangeInMeters);
        var total_count = await query.CountAsync(Cancel).ConfigureAwait(false);
        if (total_count == 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        if (PageNumber > 0) query = query.Skip(PageNumber * PageSize);
        query = query.Take(PageSize);
        var items = await query.ToArrayAsync(Cancel).ConfigureAwait(false);

        return new Page<T>(items, total_count, PageNumber, PageSize);
    }

    public async Task<T> DeleteByLocation(double Latitude, double Longitude, CancellationToken Cancel = default) =>
        await GetByLocation(Latitude, Longitude, Cancel).ConfigureAwait(false) is { } item
            ? await Delete(item, Cancel)
            : default;

    public async Task<T> DeleteByLocationInRange(
        double Latitude,
        double Longitude,
        double RangeInMeters,
        CancellationToken Cancel = default) =>
        await GetByLocationInRange(Latitude, Longitude, RangeInMeters, Cancel).ConfigureAwait(false) is { } item
            ? await Delete(item, Cancel)
            : default;
}