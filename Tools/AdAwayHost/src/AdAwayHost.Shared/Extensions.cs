namespace AdAwayHost.Shared
{
    public static class Extensions
    {
        public static long GetSizeInBytes(this string inputString)
        {
            return inputString.Length * sizeof(char);
        }
    }
}