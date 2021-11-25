using System;
using System.Collections.Generic;
using SolutionTemplate.Interfaces.Base.Entities;

namespace SolutionTemplate.DAL.Entities.Base;

/// <summary>Сущность</summary>
/// <typeparam name="TKey">Тип первичного ключа</typeparam>
public abstract class Entity<TKey> : IEntity<TKey>, IEquatable<Entity<TKey>> where TKey : IEquatable<TKey>
{
    /// <summary>Первичный ключ</summary>
    public TKey Id { get; set; }

    protected Entity() { }

    protected Entity(TKey Id) => this.Id = Id;

    public bool Equals(Entity<TKey> other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (!other.GetType().IsAssignableTo(GetType())) return false;
        if (EqualityComparer<TKey>.Default.Equals(Id, default)) 
            return ReferenceEquals(this, other);
        return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Entity<TKey>)obj);
    }

    public override int GetHashCode() =>
        EqualityComparer<TKey>.Default.Equals(Id, default) 
            ? base.GetHashCode() 
            : EqualityComparer<TKey>.Default.GetHashCode(Id);

    /// <summary>Оператор проверки на равенство двух сущностей</summary>
    /// <param name="left">Левый операнд</param><param name="right">Правый операнд</param>
    /// <returns>Истина, если значения левого и правого операнда равны</returns>
    public static bool operator ==(Entity<TKey> left, Entity<TKey> right) => Equals(left, right);

    /// <summary>Оператор проверки на неравенство двух сущностей</summary>
    /// <param name="left">Левый операнд</param><param name="right">Правый операнд</param>
    /// <returns>Истина, если значение левого операнда не равно значению правого операнда</returns>
    public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !Equals(left, right);
}

/// <summary>Сущность</summary>
public abstract class Entity : Entity<int>, IEntity
{
    protected Entity() { }

    protected Entity(int Id) : base(Id) { }
}