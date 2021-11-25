using Microsoft.EntityFrameworkCore;

namespace SolutionTemplate.DAL.Entities.Base;

[Index(nameof(LastName), nameof(FirstName), nameof(Patronymic), IsUnique = true, Name = "NameIndex")]
public abstract class Person : DescriptedEntity
{
    /// <summary>Фамилия</summary>
    public string LastName { get; set; }

    /// <summary>Имя</summary>
    public string FirstName { get; set; }

    /// <summary>Отчество</summary>
    public string Patronymic { get; set; }

    protected Person() { }

    protected Person(string LastName, string FirstName, string Patronymic)
    {
        this.LastName = LastName;
        this.FirstName = FirstName;
        this.Patronymic = Patronymic;
    }

    public override string ToString() => $"[id:{Id}] {string.Join(' ', LastName, FirstName, Patronymic)}";
}