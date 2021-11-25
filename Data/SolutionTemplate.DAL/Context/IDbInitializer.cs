using System.Threading;
using System.Threading.Tasks;

namespace SolutionTemplate.DAL.Context;

/// <summary>Инициализатор БД</summary>
public interface IDbInitializer
{
    /// <summary>Удалить БД перед инициализацией?</summary>
    bool Recreate { get; set; }

    /// <summary>Удалить БД</summary>
    /// <returns>Истина, если БД была удалена</returns>
    bool Delete();

    /// <summary>Асинхронно удалить БД</summary>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Задача удаления БД, возвращающая истину, если БД была успешно удалена</returns>
    Task<bool> DeleteAsync(CancellationToken Cancel = default);

    /// <summary>Инициализация БД</summary>
    void Initialize();

    /// <summary>Асинхронная инициализация БД</summary>
    /// <param name="Cancel">Признак отмены асинхронно операции</param>
    /// <returns>Задача инициализации БД</returns>
    Task InitializeAsync(CancellationToken Cancel = default);
}