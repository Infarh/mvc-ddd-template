using System;

namespace SolutionTemplate.Interfaces.Base.Entities;

/// <summary>Сущность, определённая во времени</summary>
/// <typeparam name="TKey">Тип первичного ключа</typeparam>
public interface ITimedEntity<out TKey> : IEntity<TKey>
{
    /// <summary>Время</summary>
    DateTimeOffset Time { get; }
}

/// <summary>Сущность, определённая во времени</summary>
public interface ITimedEntity : ITimedEntity<int>, IEntity { }