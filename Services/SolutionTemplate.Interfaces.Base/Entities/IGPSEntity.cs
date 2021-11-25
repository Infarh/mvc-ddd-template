namespace SolutionTemplate.Interfaces.Base.Entities;

/// <summary>Сущность, обеспечивающая возможность позиционирования</summary>
/// <typeparam name="TKey">Тип первичного ключа</typeparam>
public interface IGPSEntity<out TKey> : IEntity<TKey>
{
    /// <summary>Широта</summary>
    double Latitude { get; set; }

    /// <summary>Долгота</summary>
    double Longitude { get; set; }
}

/// <summary>Сущность, обеспечивающая возможность позиционирования</summary>
public interface IGPSEntity : IGPSEntity<int>, IEntity { }