namespace Mobile.Models.ViewModels
{
    public class AttachmentViewModel
    {
        public string FileName { get; set; }

        public string DisplayName
        {
            get
            {
                if (FileName.Contains("extra"))
                {
                    return "Extra";
                }

                // Extract the display name from the file name (e.g., "left_front.jpg" -> "Left Front")
                var nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(FileName);
                var displayName = nameWithoutExtension.Replace("photo-", "").Replace("_", " ").Replace("-", " ");
                // Remove digits (sometimes used to differentiate "OTHER" photos)
                displayName = System.Text.RegularExpressions.Regex.Replace(displayName, @"\d", "");
                return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(displayName);
            }
        }

        public string Path { get; set; }

        public DateTime DateEntered { get; set; }

        public bool IsEncrypted { get; set; }
    }
}
