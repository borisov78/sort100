using System.IO;
using System.Text;

namespace Sort100.Helpers
{
    public static class FileHelper
    {
        public static Encoding DetectEncoding(string filePath)
        {
            using var reader = new StreamReader(filePath, Encoding.Default, true);
            reader.Peek();
            return reader.CurrentEncoding;
        }

        public static void SafeDeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // TODO Log
            }
        }

        public static void SafeDeleteDirectory(string directoryPath)
        {
            try
            {
                Directory.Delete(directoryPath);
            }
            catch
            {
                // TODO Log
            }
        }
        
        
        
    }
}