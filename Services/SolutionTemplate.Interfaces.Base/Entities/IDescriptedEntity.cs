namespace SolutionTemplate.Interfaces.Base.Entities;

/// <summary>Сущность, обладающая описанием</summary>
/// <typeparam name="TKey">Тип первичного ключа</typeparam>
public interface IDescriptedEntity<out TKey> : IEntity<TKey>
{
    /// <summary>Описание</summary>
    string Description { get; }
}

/// <summary>Сущность, обладающая описанием</summary>
public interface IDescriptedEntity : IDescriptedEntity<int>, IEntity { }