using System;
using SolutionTemplate.Interfaces.Base.Entities;

namespace SolutionTemplate.DAL.Entities.Base;

/// <summary>Именованная сущность с описанием</summary>
/// <typeparam name="TKey">Тип первичного ключа</typeparam>
public abstract class NamedDescriptedEntity<TKey> : NamedEntity<TKey>, IDescriptedEntity<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>Описание</summary>
    public string Description { get; set; }
}

/// <summary>Именованная сущность с описанием</summary>
public abstract class NamedDescriptedEntity : NamedDescriptedEntity<int>, IDescriptedEntity { }