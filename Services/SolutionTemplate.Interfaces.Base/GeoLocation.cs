using System;
using System.Globalization;
using static System.Math;

namespace SolutionTemplate.Interfaces.Base;

/// <summary>Географическое положение</summary>
public readonly struct GeoLocation
{
    /// <summary>Радиус Земли в метрах</summary>
    public const double EarthRadius = 6_378_137d;

    /// <summary>Преобразование градусов в радианы</summary>
    internal const double ToRad = PI / 180;

    /// <summary>Широта в градусах</summary>
    public double Latitude { get; init; }

    /// <summary>Долгота в градусах</summary>
    public double Longitude { get; init; }

    /// <summary>Градусы широты</summary>
    public int LatAngle => (int)Latitude;

    /// <summary>Минуты широты</summary>
    public int LatMinutes => (int)((Latitude - LatAngle) * 60);

    /// <summary>Секунды широты</summary>
    public double LatSeconds => ((Latitude - LatAngle) * 60 - LatMinutes) * 60;

    /// <summary>Градусы долготы</summary>
    public int LonAngle => (int)Longitude;

    /// <summary>Минуты долготы</summary>
    public int LonMinutes => (int)((Longitude - LonAngle) * 60);

    /// <summary>Секунды долготы</summary>
    public double LonSeconds => ((Longitude - LonAngle) * 60 - LonMinutes) * 60;

    /// <summary>Инициализация нового географического положения</summary>
    /// <param name="Latitude">Широта в градусах</param>
    /// <param name="Longitude">Долгота в градусах</param>
    public GeoLocation(double Latitude, double Longitude)
    {
        this.Latitude = Latitude;
        this.Longitude = Longitude;
    }

    /// <summary>Инициализация нового географического положения из кортежа положения</summary>
    /// <param name="Point">Кортеж со значениями широты и долготы</param>
    public GeoLocation((double Latitude, double Longitude) Point) => (Latitude, Longitude) = Point;

    /// <summary>Деконструктор географического положения на широту и долготу</summary>
    /// <param name="latitude">Широта</param>
    /// <param name="longitude">Долгота</param>
    public void Deconstruct(out double latitude, out double longitude)
    {
        latitude = Latitude;
        longitude = Longitude;
    }

    /// <summary>Вычисление дистанции до другого географического положения в метрах</summary>
    /// <param name="Location">Вторая точка географического положения</param>
    /// <returns>Расстояние до указанной точки в метрах</returns>
    public double DistanceTo(GeoLocation Location)
    {
        var lat1 = Latitude * ToRad;
        var lon1 = Longitude * ToRad;

        var lat2 = Location.Latitude * ToRad;
        var lon2 = Location.Longitude * ToRad;

        var d_lat = lat2 - lat1;
        var d_lon = lon2 - lon1;

        var sin_lat = Sin(d_lat / 2);
        var sin_lon = Sin(d_lon / 2);

        var a = sin_lat * sin_lat + Cos(lat2) * Cos(lat1) * sin_lon * sin_lon;
        return 2 * Atan2(Sqrt(a), Sqrt(1 - a)) * EarthRadius;
    }

    /// <summary>Определение курса в направлении на указанную географическую точку (в градусах)</summary>
    /// <param name="Location">Точка назначения</param>
    /// <returns>Угол курса в градусах</returns>
    public double HeadingTo(GeoLocation Location)
    {
        var lat1 = Latitude * ToRad;
        var lon1 = Longitude * ToRad;

        var lat2 = Location.Latitude * ToRad;
        var lon2 = Location.Longitude * ToRad;

        var d_lon = lon2 - lon1;
        var y = Sin(d_lon) * Cos(lat2);
        var x = Cos(lat1) * Sin(lat2) - Sin(lat1) * Cos(lat2) * Cos(d_lon);
        return (Atan2(y, x) / ToRad + 360) % 360;
    }

    /// <summary>Определение точки места назначения по курсу в градусах и дистанции в метрах</summary>
    /// <param name="Heading">Угол курса в градусах</param>
    /// <param name="Distance">Дистанция в метрах</param>
    /// <returns>Новая точка места назначения на заданном удалении и с заданным курсом</returns>
    public GeoLocation Destination(double Heading, double Distance)
    {
        var latitude = Latitude * ToRad;
        var longitude = Longitude * ToRad;
        if (Heading < 0 || Heading > 360) Heading = (Heading + 360) % 360;
        Heading *= ToRad;

        Distance /= EarthRadius;

        var sin_lat = Sin(latitude);
        var cos_lat = Cos(latitude);
        var sin_d = Sin(Distance);
        var cos_d = Cos(Distance);

        var sin_latitude2 = sin_lat * cos_d + cos_lat * sin_d * Cos(Heading);
        var longitude2 = longitude + Atan2(Sin(Heading) * sin_d * cos_lat, cos_d - sin_lat * sin_latitude2);
        return new GeoLocation(Asin(sin_latitude2) / ToRad, (longitude2 / ToRad + 540) % 360 - 180);
    }

    public override string ToString()
    {
        var lat = Latitude;
        var lon = Longitude;

        var lat_sign = Sign(lat);
        var lon_sign = Sign(lon);

        lat = Abs(lat);
        lon = Abs(lon);

        var lat_angle = (int)lat;
        var lon_angle = (int)lon;

        lat -= lat_angle;
        lon -= lon_angle;

        lat *= 60;
        lon *= 60;

        var lat_min = (int)lat;
        var lon_min = (int)lon;

        lat -= lat_min;
        lon -= lon_min;

        lat *= 60;
        lon *= 60;

        FormattableString result = $"{lat_angle}°{lat_min:00}'{lat:00.############}''{(lat_sign >= 0 ? "N" : "S")}, {lon_angle}°{lon_min:00}'{lon:00.############}''{(lon_sign >= 0 ? "E" : "W")}";

        return result.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Оператор неявного приведения кортежа географических координат в структуру географического положения</summary>
    /// <param name="Point">Кортеж, содержащий широту и долготу</param>
    public static implicit operator GeoLocation((double Latitude, double Longitude) Point) => new(Point);

    /// <summary>Оператор неявного преобразования географического положения в кортеж, содержащий широту и долготу</summary>
    /// <param name="Location">Структура географического положения</param>
    public static implicit operator (double Latitude, double Longitude)(GeoLocation Location) => (Location.Latitude, Location.Longitude);
}