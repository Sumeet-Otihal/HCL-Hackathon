namespace HotelBooking.Core.Helpers;

public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string HotelAdmin = "HotelAdmin";
    public const string User = "User";
    public const string SuperAdminOrHotelAdmin = "SuperAdmin,HotelAdmin";
    public const string AnyAuthenticated = "SuperAdmin,HotelAdmin,User";
}
