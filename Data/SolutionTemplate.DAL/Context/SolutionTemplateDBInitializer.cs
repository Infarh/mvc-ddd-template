using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SolutionTemplate.DAL.Context;

public class SolutionTemplateDBInitializer : IDbInitializer
{
    private readonly SolutionTemplateDB _db;
    private readonly ILogger<SolutionTemplateDBInitializer> _Logger;

    public bool Recreate { get; set; }

    public SolutionTemplateDBInitializer(SolutionTemplateDB db, ILogger<SolutionTemplateDBInitializer> Logger)
    {
        _db = db;
        _Logger = Logger;
    }

    public bool Delete()
    {
        if (!_db.Database.EnsureDeleted()) return false;
        _Logger.LogInformation("БД удалена");
        return true;
    }

    public async Task<bool> DeleteAsync(CancellationToken Cancel = default)
    {
        if (!await _db.Database.EnsureDeletedAsync(Cancel).ConfigureAwait(false)) return false;
        _Logger.LogInformation("БД удалена");
        return true;
    }

    public void Initialize()
    {
        var db = _db.Database;

        if (Recreate) Delete();

        var pending_migrations = db.GetPendingMigrations().ToArray();
        var applied_migrations = db.GetAppliedMigrations().ToArray();
        if (applied_migrations.Length == 0 && pending_migrations.Length == 0)
        {
            if (db.EnsureCreated())
                _Logger.LogInformation("БД успешно создана");
        }
        else if (pending_migrations.Length > 0)
        {
            _Logger.LogInformation(
                "БД существует. Миграций применено {0}. Требуется применить миграций {1}",
                applied_migrations.Length, pending_migrations.Length);
            if (applied_migrations.Length > 0)
                _Logger.LogInformation("Применённые миграции: {0}", string.Join(",", applied_migrations));

            db.Migrate();
            _Logger.LogInformation("Применённые миграции: {0}", string.Join(",", pending_migrations));
        }

        _Logger.LogInformation("Базовая инициализация экземпляра БД выполнена");
    }

    public async Task InitializeAsync(CancellationToken Cancel = default)
    {
        var db = _db.Database;

        if (Recreate) await DeleteAsync(Cancel).ConfigureAwait(false);

        var pending_migrations = (await db.GetPendingMigrationsAsync(Cancel)).ToArray();
        var applied_migrations = (await db.GetAppliedMigrationsAsync(Cancel)).ToArray();
        if (applied_migrations.Length == 0 && pending_migrations.Length == 0)
        {
            if (await db.EnsureCreatedAsync(Cancel))
                _Logger.LogInformation("БД успешно создана");
        }
        else if (pending_migrations.Length > 0)
        {
            _Logger.LogInformation(
                "БД существует. Миграций применено {0}. Требуется применить миграций {1}",
                applied_migrations.Length, pending_migrations.Length);
            if (applied_migrations.Length > 0)
                _Logger.LogInformation("Применённые миграции: {0}", string.Join(",", applied_migrations));

            await db.MigrateAsync(Cancel);

            _Logger.LogInformation("Применённые миграции: {0}", string.Join(",", pending_migrations));
        }

        _Logger.LogInformation("Базовая инициализация экземпляра БД выполнена");
    }
}