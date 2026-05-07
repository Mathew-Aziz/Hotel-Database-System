namespace HotelManagementSystem.WinForms.Utils;

public static class Validation
{
    public static bool IsBlank(string? s) => string.IsNullOrWhiteSpace(s);

    public static bool IsPositiveDecimal(string text, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;
        if (!decimal.TryParse(text, out value)) return false;
        return value > 0;
    }
}