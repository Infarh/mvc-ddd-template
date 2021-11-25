using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SolutionTemplate.Interfaces.Base.Entities;

namespace SolutionTemplate.Interfaces.Base.Repositories;

/// <summary>Репозиторий сущностей</summary>
/// <typeparam name="T">Тип сущности, хранимой в репозитории</typeparam>
/// <typeparam name="TKey">Тип первичного ключа</typeparam>
public interface IRepository<T, in TKey> where T : IEntity<TKey>
{
    /// <summary>Проверка репозитория на пустоту</summary>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Истина, если в репозитории нет ни одной сущности</returns>
    Task<bool> IsEmpty(CancellationToken Cancel = default);

    /// <summary>Существует ли сущность с указанным идентификатором</summary>
    /// <param name="Id">Проверяемый идентификатор сущности</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Истина, если сущность с указанным идентификатором существует в репозитории</returns>
    Task<bool> ExistId(TKey Id, CancellationToken Cancel = default);

    /// <summary>Существует ли в репозитории указанная сущность</summary>
    /// <param name="item">Проверяемая сущность</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Истина, если указанная сущность существует в репозитории</returns>
    Task<bool> Exist(T item, CancellationToken Cancel = default);

    /// <summary>Получить число хранимых сущностей</summary>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    Task<int> GetCount(CancellationToken Cancel = default);

    /// <summary>Извлечь все сущности из репозитория</summary>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Перечисление всех сущностей репозитория</returns>
    Task<IEnumerable<T>> GetAll(CancellationToken Cancel = default);

    /// <summary>Получить набор сущностей из репозитория в указанном количестве, предварительно пропустив некоторое количество</summary>
    /// <param name="Skip">Число предварительно пропускаемых сущностей</param>
    /// <param name="Count">Число извлекаемых из репозитория сущностей</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Перечисление полученных из репозитория сущностей</returns>
    Task<IEnumerable<T>> Get(int Skip, int Count, CancellationToken Cancel = default);

    /// <summary>Получить страницу с сущностями из репозитория</summary>
    /// <param name="PageNumber">Номер страницы начиная с нуля</param>
    /// <param name="PageSize">Размер страницы</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Страница с сущностями из репозитория</returns>
    Task<IPage<T>> GetPage(int PageNumber, int PageSize, CancellationToken Cancel = default);

    /// <summary>Получить сущность по указанному идентификатору</summary>
    /// <param name="Id">Идентификатор извлекаемой сущности</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Сущность с указанным идентификатором в случае её наличия и null, если сущность отсутствует</returns>
    Task<T> GetById(int Id, CancellationToken Cancel = default);

    /// <summary>Добавление сущности в репозиторий</summary>
    /// <param name="item">Добавляемая в репозиторий сущность</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Добавленная в репозиторий сущность</returns>
    Task<T> Add(T item, CancellationToken Cancel = default);

    /// <summary>Добавление перечисленных сущностей в репозиторий</summary>
    /// <param name="items">Перечисление добавляемых в репозиторий сущностей</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Задача, завершающаяся при завершении операции добавления сущностей</returns>
    Task AddRange(IEnumerable<T> items, CancellationToken Cancel = default);

    /// <summary>Добавление сущности в репозиторий с помощью фабричного метода</summary>
    /// <param name="ItemFactory">Метод, формирующий добавляемую в репозиторий сущность</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Добавленная в репозиторий сущность</returns>
    Task<T> AddAsync(Func<T> ItemFactory, CancellationToken Cancel = default) => Add(ItemFactory(), Cancel);

    /// <summary>Обновление сущности в репозитории</summary>
    /// <param name="item">Сущность, хранящая в себе информацию, которую надо обновить в репозитории</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Сущность из репозитория с обновлёнными данными</returns>
    Task<T> Update(T item, CancellationToken Cancel = default);

    /// <summary>Обновление сущности в репозитории</summary>
    /// <param name="id">Идентификатор обновляемой сущности</param>
    /// <param name="ItemUpdated">Метод обновления информации в заданной сущности</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Сущность из репозитория с обновлёнными данными</returns>
    Task<T> UpdateAsync(TKey id, Action<T> ItemUpdated, CancellationToken Cancel = default);

    /// <summary>Обновление перечисленных сущностей</summary>
    /// <param name="items">Перечисление сущностей, информацию из которых надо обновить в репозитории</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Задача, завершаемая при завершении операции обновления сущностей</returns>
    Task UpdateRange(IEnumerable<T> items, CancellationToken Cancel = default);

    /// <summary>Удаление сущности из репозитория</summary>
    /// <param name="item">Удаляемая из репозитория сущность</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Удалённая из репозитория сущность</returns>
    Task<T> Delete(T item, CancellationToken Cancel = default);

    /// <summary>Удаление перечисления сущностей из репозитория</summary>
    /// <param name="items">Перечисление удаляемых сущностей</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Задача, завершаемая при завершении операции удаления сущностей</returns>
    Task DeleteRange(IEnumerable<T> items, CancellationToken Cancel = default);

    /// <summary>Удаление сущности по заданному идентификатору</summary>
    /// <param name="id">Идентификатор сущности, которую надо удалить</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Удалённая из репозитория сущность</returns>
    Task<T> DeleteById(TKey id, CancellationToken Cancel = default);
}

/// <summary>Репозиторий сущностей</summary>
public interface IRepository<T> : IRepository<T, int> where T : IEntity<int> { }