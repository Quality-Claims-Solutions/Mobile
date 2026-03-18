namespace Mobile.Utilities
{
    public static class ExtensionsUtility
    {
        public static string RemoveIllegalPathChars(this string @string)
        {
            foreach (var illegal in Path.GetInvalidFileNameChars())
            {
                @string = @string.Replace(illegal.ToString(), "");
            }
            @string = @string.Replace("$", "");
            return @string;
        }
    }
}
