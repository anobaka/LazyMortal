namespace Bootstrap.Extensions
{
    public static class IntExtensions
    {
        public static string ToHex(this int value, int totalWidth = 6) => value.ToString("X").PadLeft(totalWidth);
        public static string ToHex(this uint value, int totalWidth = 6) => value.ToString("X").PadLeft(totalWidth);
    }
}