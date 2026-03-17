namespace Mobile.Utilities
{
    public class URIUtility
    {
        public static bool HasURIBrand(Uri uri)
        {
            if (uri.Host.Contains("-app"))
            {
                return true;
            }

            return false;
        }

        public static string GetURIBranding(Uri uri)
        {
            if (HasURIBrand(uri))
            {
                var end = uri.Host.IndexOf("-app");
                return uri.Host.Substring(0, end);
            }
#if DEBUG
            return "hertz"; // set custom test pwa here
#endif  
            return "aba"; // default to ABA for demo purposes
        }
    }
}
