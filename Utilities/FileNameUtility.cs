namespace Mobile.Utilities
{
    public static class FileNameUtility
    {
        public static string GetUniqueFileName(string rootFolder, string fileName, string claimNumber = "")
        {
            var now = DateTime.Now;
            var dateFolder = $"{now.Year}\\{now.Month}\\{now.Day}";
            if (Directory.Exists(Path.Combine(rootFolder, dateFolder)) == false)
                Directory.CreateDirectory(Path.Combine(rootFolder, dateFolder));

            fileName = fileName.RemoveIllegalPathChars();

            var noExt = Path.GetFileNameWithoutExtension(fileName);
            if (!string.IsNullOrEmpty(claimNumber))
            {
                noExt = claimNumber.Trim() + "_" + noExt.TrimStart('_');
            }

            var ext = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(ext))
            {
                ext = ".jpg";
            }

            var rand = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var path = Path.Combine(dateFolder, noExt + "." + rand + ext);
            // make sure path isn't too long
            var totalLength = rootFolder.Length + path.Length + 1;
            if (totalLength > 259) // path must be less than 260 characters
            {
                var excess = totalLength - 259;
                noExt = noExt.Substring(0, noExt.Length - excess);
                path = Path.Combine(dateFolder, noExt + "." + rand + ext);
            }
            while (File.Exists(Path.Combine(rootFolder, path)))
            {
                rand = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                path = Path.Combine(dateFolder, noExt + "." + rand + ext);
            }
            return path;
        }
    }
}
