using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SolutionTemplate.Interfaces.Base.Entities;

namespace SolutionTemplate.Interfaces.Base.Repositories;

/// <summary>Репозиторий сущностей, обладающих указанием времени</summary>
/// <typeparam name="T">Тип сущностей</typeparam>
/// <typeparam name="TKey">Тип первичного ключа сущности</typeparam>
public interface ITimedRepository<T, in TKey> : IRepository<T, TKey> where T : ITimedEntity<TKey>
{
    /// <summary>Проверка на существование сущностей, время которых больше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Истина, если в репозитории существуют сущности, время которых больше, чем указанное</returns>
    Task<bool> ExistGreaterThenTime(DateTimeOffset ReferenceTime, CancellationToken Cancel = default);

    /// <summary>Получить число сущностей, время которых больше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Число сущностей, время которых больше, Чем указанное</returns>
    Task<int> GetCountGreaterThenTime(DateTimeOffset ReferenceTime, CancellationToken Cancel = default);

    /// <summary>Получить сущности из репозитория, время которых больше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Все сущности, время которых больше, чем указанное</returns>
    Task<IEnumerable<T>> GetAllGreaterThenTime(DateTimeOffset ReferenceTime, CancellationToken Cancel = default);

    /// <summary>Получить заданное число сущностей из репозитория, время которых больше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="Skip">Пропускаемое количество сущностей в начале выборки</param>
    /// <param name="Count">Извлекаемое число сущностей</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Все сущности, время которых больше, чем указанное</returns>
    Task<IEnumerable<T>> GetGreaterThenTime(DateTimeOffset ReferenceTime, int Skip, int Count, CancellationToken Cancel = default);

    /// <summary>Получить страницу с сущностями, время которых больше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="PageIndex">Индекс запрашиваемой страницы (начиная с 0)</param>
    /// <param name="PageSize">Размер страницы</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Страница с сущностями, время которых больше, чем указанное</returns>
    Task<IPage<T>> GetPageGreaterThenTime(DateTimeOffset ReferenceTime, int PageIndex, int PageSize, CancellationToken Cancel = default);

    /// <summary>Проверка на существование сущностей, время которых меньше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Истина, если в репозитории существуют сущности, время которых меньше, чем указанное</returns>
    Task<bool> ExistLessThenTime(DateTimeOffset ReferenceTime, CancellationToken Cancel = default);

    /// <summary>Получить число сущностей, время которых меньше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Число сущностей, время которых меньше, Чем указанное</returns>
    Task<int> GetCountLessThenTime(DateTimeOffset ReferenceTime, CancellationToken Cancel = default);

    /// <summary>Получить сущности из репозитория, время которых меньше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Все сущности, время которых меньше, чем указанное</returns>
    Task<IEnumerable<T>> GetAllLessThenTime(DateTimeOffset ReferenceTime, CancellationToken Cancel = default);

    /// <summary>Получить заданное число сущностей из репозитория, время которых меньше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="Skip">Пропускаемое количество сущностей в начале выборки</param>
    /// <param name="Count">Извлекаемое число сущностей</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Все сущности, время которых меньше, чем указанное</returns>
    Task<IEnumerable<T>> GetLessThenTime(DateTimeOffset ReferenceTime, int Skip, int Count, CancellationToken Cancel = default);

    /// <summary>Получить страницу с сущностями, время которых меньше, чем указанное</summary>
    /// <param name="ReferenceTime">Заданная временная отметка</param>
    /// <param name="PageIndex">Индекс запрашиваемой страницы (начиная с 0)</param>
    /// <param name="PageSize">Размер страницы</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Страница с сущностями, время которых меньше, чем указанное</returns>
    Task<IPage<T>> GetPageLessThenTime(DateTimeOffset ReferenceTime, int PageIndex, int PageSize, CancellationToken Cancel = default);

    /// <summary>Получение всех сущностей, время которых находится в заданном интервале</summary>
    /// <param name="StartTime">Время начала интервала поиска</param>
    /// <param name="EndTime">Время конца интервала поиска</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Перечисление всех сущностей, время которых попадает в указанный интервал</returns>
    Task<IEnumerable<T>> GetAllInTimeInterval(DateTimeOffset StartTime, DateTimeOffset EndTime, CancellationToken Cancel = default);

    /// <summary>Получить страницу с сущностями, время которых попадает в указанный интервал</summary>
    /// <param name="StartTime">Время начала интервала поиска</param>
    /// <param name="EndTime">Время конца интервала поиска</param>
    /// <param name="PageIndex">Индекс запрашиваемой страницы (начиная с 0)</param>
    /// <param name="PageSize">Размер страницы</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Страница с сущностями, время которых попадает в указанный интервал</returns>
    Task<IPage<T>> GetInTimeInterval(DateTimeOffset StartTime, DateTimeOffset EndTime, int PageIndex, int PageSize, CancellationToken Cancel = default);
}

/// <summary>Репозиторий сущностей, обладающих указанием времени</summary>
/// <typeparam name="T">Тип сущностей</typeparam>
public interface ITimedRepository<T> : ITimedRepository<T, int> where T : ITimedEntity<int> { }