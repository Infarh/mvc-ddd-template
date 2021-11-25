using System;
using SolutionTemplate.Interfaces.Base.Entities;

namespace SolutionTemplate.DAL.Entities.Base;

/// <summary>Сущность, снабжённая комментарием</summary>
/// <typeparam name="TKey">Тип первичного ключа</typeparam>
public abstract class DescriptedEntity<TKey> : Entity<TKey>, IDescriptedEntity<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>Комментарий</summary>
    public string Description { get; set; }
}

/// <summary>Сущность, снабжённая комментарием</summary>
public abstract class DescriptedEntity : DescriptedEntity<int>, IDescriptedEntity { }