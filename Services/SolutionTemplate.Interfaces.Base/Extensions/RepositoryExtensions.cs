using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.Interfaces.Base.Extensions;

public static class RepositoryExtensions
{
    /// <summary>Перечисление всех страниц сущностей репозитория</summary>
    /// <typeparam name="T">Тип сущности репозитория</typeparam>
    /// <typeparam name="TKey">Тип первичного ключа сущности репозитория</typeparam>
    /// <param name="repository">Репозиторий, перечисление страниц которого надо выполнить</param>
    /// <param name="PageSize">Размер страницы</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Последовательное перечисление страниц сущностей репозитория</returns>
    /// <exception cref="ArgumentOutOfRangeException">Если размер страницы меньше, либо равен 0</exception>
    public static async IAsyncEnumerable<IPage<T>> EnumPages<T, TKey>(
        this IRepository<T, TKey> repository,
        int PageSize, 
        [EnumeratorCancellation] CancellationToken Cancel = default) 
        where T : IEntity<TKey>
    {
        if (repository is null) throw new ArgumentNullException(nameof(repository));
        if (PageSize <= 0) throw new ArgumentOutOfRangeException(nameof(PageSize), PageSize, "Размер страницы должен быть больше нуля");

        IPage<T> page;
        var index = 0;
        do
        {
            page = await repository.GetPage(index++, PageSize, Cancel).ConfigureAwait(false);
            yield return page;
        }
        while (page.HasNextPage);
    }
}