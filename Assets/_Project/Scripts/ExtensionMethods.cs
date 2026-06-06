public static class ExtensionMethods
{
    public static bool IsWithinRange(this int value, int min, int max)
    {
        return value >= min && value < max;
    }
}
