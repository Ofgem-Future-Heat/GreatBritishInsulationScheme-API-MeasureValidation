namespace Ofgem.API.GBI.MeasureValidation.Application.Extensions
{
    public static class StringExtensions
    {
        public static bool CaseInsensitiveContainsInList(this IEnumerable<string> source, string? value)
        {
            return source.Contains(value, StringComparer.OrdinalIgnoreCase);
        }

        public static bool CaseInsensitiveEquals(this string? source, string? value)
        {
            if (source == null || string.IsNullOrWhiteSpace(value)) return false;
            return source.Trim().Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
