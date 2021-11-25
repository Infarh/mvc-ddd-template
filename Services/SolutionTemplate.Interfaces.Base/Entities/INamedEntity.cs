using System.ComponentModel.DataAnnotations;

namespace SolutionTemplate.Interfaces.Base.Entities;

/// <summary>Именованная сущность</summary>
/// <typeparam name="TKey">Тип первичного ключа</typeparam>
public interface INamedEntity<out TKey> : IEntity<TKey>
{
    /// <summary>Имя</summary>
    [Required]
    string Name { get; }
}

/// <summary>Именованная сущность</summary>
public interface INamedEntity : INamedEntity<int>, IEntity { }