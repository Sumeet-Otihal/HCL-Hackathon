namespace HotelBooking.Core.Helpers;

public static class ReservationNumberGenerator
{
    private static readonly Random _random = new();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = new string(Enumerable.Repeat(Chars, 4)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
        return $"RES-{datePart}-{randomPart}";
    }
}
