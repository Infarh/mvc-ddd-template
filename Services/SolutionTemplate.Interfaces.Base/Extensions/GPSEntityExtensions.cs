using System;
using System.Collections.Generic;
using System.Linq;
using SolutionTemplate.Interfaces.Base.Entities;

namespace SolutionTemplate.Interfaces.Base.Extensions;

/// <summary>Методы-расширения для сущностей с геопозиционированием</summary>
public static class GPSEntityExtensions
{
    /// <summary>Упорядочить по удалению от указанной точки и выбрать лишь те элементы, что попадают в указанный радиус</summary>
    /// <typeparam name="T">Тип элементов</typeparam>
    /// <param name="query">Исходный запрос</param>
    /// <param name="Latitude">Широта опорной точки</param>
    /// <param name="Longitude">Долгота опорной точки</param>
    /// <param name="Range">Ограничивающий радиус в метрах</param>
    /// <returns>Запрос элементов вокруг указанной точки в заданном радиусе</returns>
    public static IQueryable<T> OrderByDistanceInRange<T>(
        this IQueryable<T> query,
        double Latitude,
        double Longitude,
        double Range)
        where T : IGPSEntity
    {
        var lat = Latitude * GeoLocation.ToRad;
        var lon = Longitude * GeoLocation.ToRad;

        return
            from item in query
            let d_lat = item.Latitude * GeoLocation.ToRad - lat
            let d_lon = item.Longitude * GeoLocation.ToRad - lon
            let sin_lat = Math.Sin(d_lat / 2)
            let sin_lon = Math.Sin(d_lon / 2)
            let a = sin_lat * sin_lat + Math.Cos(item.Latitude * GeoLocation.ToRad) * Math.Cos(lat) * sin_lon * sin_lon
            let r = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)) * GeoLocation.EarthRadius
            where r <= Range
            orderby r
            select item;
    }

    /// <summary>Отсортировать по увеличению дальности от указанной точки</summary>
    /// <typeparam name="T">Тип элемента, имеющего географические координаты</typeparam>
    /// <param name="query">Запрос</param>
    /// <param name="Latitude">Широта указанной точки</param>
    /// <param name="Longitude">Долгота указанной точки</param>
    /// <returns>Запрос, содержащий последовательность элементов, упорядоченную по удалению от указанной точки</returns>
    public static IQueryable<T> OrderByDistance<T>(this IQueryable<T> query, double Latitude, double Longitude)
        where T : IGPSEntity =>
        query.OrderBy(item => (item.Latitude - Latitude) * (item.Latitude - Latitude) + (item.Longitude - Longitude) * (item.Longitude - Longitude));

    /// <summary>Получить ближайший объект к указанной точке</summary>
    /// <typeparam name="T">Тип элемента, имеющего географические координаты</typeparam>
    /// <param name="query">Запрос</param>
    /// <param name="Latitude">Широта указанной точки</param>
    /// <param name="Longitude">Долгота указанной точки</param>
    /// <returns>Первый ближайший элемент к указанной точке</returns>
    public static T Closest<T>(this IQueryable<T> query, double Latitude, double Longitude)
        where T : IGPSEntity => query
       .OrderByDistance(Latitude, Longitude)
       .First();

    /// <summary>Получить ближайший объект к указанной точке</summary>
    /// <typeparam name="T">Тип элемента, имеющего географические координаты</typeparam>
    /// <param name="query">Запрос</param>
    /// <param name="Latitude">Широта указанной точки</param>
    /// <param name="Longitude">Долгота указанной точки</param>
    /// <returns>Первый ближайший элемент к указанной точке</returns>
    public static T? ClosestOrDefault<T>(this IQueryable<T> query, double Latitude, double Longitude)
        where T : IGPSEntity => query
       .OrderByDistance(Latitude, Longitude)
       .FirstOrDefault();

    /// <summary>Отсортировать по увеличению дальности от указанной точки</summary>
    /// <typeparam name="T">Тип элемента, имеющего географические координаты</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="Latitude">Широта указанной точки</param>
    /// <param name="Longitude">Долгота указанной точки</param>
    /// <returns>Последовательность элементов, содержащий последовательность элементов, упорядоченную по удалению от указанной точки</returns>
    public static IEnumerable<T> OrderByDistance<T>(this IEnumerable<T> items, double Latitude, double Longitude)
        where T : IGPSEntity =>
        items.OrderBy(item => (item.Latitude - Latitude) * (item.Latitude - Latitude) + (item.Longitude - Longitude) * (item.Longitude - Longitude));

    /// <summary>Получить ближайший объект к указанной точке</summary>
    /// <typeparam name="T">Тип элемента, имеющего географические координаты</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="Latitude">Широта указанной точки</param>
    /// <param name="Longitude">Долгота указанной точки</param>
    /// <returns>Первый ближайший элемент к указанной точке</returns>
    public static T Closest<T>(this IEnumerable<T> items, double Latitude, double Longitude)
        where T : IGPSEntity => items
       .OrderByDistance(Latitude, Longitude)
       .First();

    /// <summary>Получить ближайший объект к указанной точке</summary>
    /// <typeparam name="T">Тип элемента, имеющего географические координаты</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="Latitude">Широта указанной точки</param>
    /// <param name="Longitude">Долгота указанной точки</param>
    /// <returns>Первый ближайший элемент к указанной точке</returns>
    public static T? ClosestOrDefault<T>(this IEnumerable<T> items, double Latitude, double Longitude)
        where T : IGPSEntity => items
       .OrderByDistance(Latitude, Longitude)
       .FirstOrDefault();
}