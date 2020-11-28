namespace AdAwayHost.Shared
{
  public static class Extensions
  {
    public static long GetSizeInBytes(this string inputString) =>
      inputString.Length * sizeof(char);
  }
}
