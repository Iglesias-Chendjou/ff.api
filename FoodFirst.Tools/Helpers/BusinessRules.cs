namespace FoodFirst.Tools.Helpers;

public static class BusinessRules
{
    public const int OrderCutoffHour = 17;

    public static readonly (decimal Latitude, decimal Longitude) WarehouseBrussels = (50.8503m, 4.3517m);

    private static readonly TimeZoneInfo BrusselsTz =
        TimeZoneInfo.TryFindSystemTimeZoneById("Romance Standard Time", out var tz)
            ? tz
            : TimeZoneInfo.FindSystemTimeZoneById("Europe/Brussels");

    public static DateTime ComputeExpectedDeliveryDate(DateTime nowUtc)
    {
        var brussels = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, BrusselsTz);
        var targetDate = brussels.Hour >= OrderCutoffHour
            ? brussels.Date.AddDays(1)
            : brussels.Date;
        var targetLocal = targetDate.AddHours(12);
        return TimeZoneInfo.ConvertTimeToUtc(targetLocal, BrusselsTz);
    }

    public static double HaversineKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadiusKm = 6371.0;
        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double ToRadians(double deg) => deg * Math.PI / 180.0;
}
