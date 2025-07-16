public static class StringExtensions
{
    public static string ReplacePath(this string str) => str.Replace("$", AppDomain.CurrentDomain.BaseDirectory).Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
}
