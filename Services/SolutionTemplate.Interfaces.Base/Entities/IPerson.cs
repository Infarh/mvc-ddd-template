namespace SolutionTemplate.Interfaces.Base.Entities;

/// <summary>Персона</summary>
/// <typeparam name="TKey"></typeparam>
public interface IPerson<out TKey> : IEntity<TKey>
{
    /// <summary>Фамилия</summary>
    public string LastName { get; set; }

    /// <summary>Имя</summary>
    public string FirstName { get; set; }

    /// <summary>Отчество</summary>
    public string Patronymic { get; set; }
}