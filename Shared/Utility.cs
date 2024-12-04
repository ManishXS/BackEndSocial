namespace BackEnd.Shared
{
    public static class Utility
    {
        public static string GetFileNameFromUrl(string url)
        {

            Uri uri = new Uri(url);
            string fileName = System.IO.Path.GetFileName(uri.LocalPath);

            return fileName;
        }

        public static string GetLastPartAfterHyphen(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty; 
            }

            string[] parts = input.Split('-');

            return parts[^1];
        }
    }
}
