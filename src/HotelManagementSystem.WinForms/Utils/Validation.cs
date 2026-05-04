namespace HotelManagementSystem.WinForms.Utils;

public static class Validation
{
    public static bool IsBlank(string? s) => string.IsNullOrWhiteSpace(s);
}
