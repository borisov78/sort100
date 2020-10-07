using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Sort100.Common
{
    public static class ResourceUtil
    {
        public static string GetStringFromResources(Assembly assembly, string resourceName)
        {
            var embeddedProvider = new ManifestEmbeddedFileProvider(assembly);
            using var resourceReader = embeddedProvider.GetFileInfo(resourceName).CreateReadStream();
            using var stringReader = new StreamReader(resourceReader, true);
            return stringReader.ReadToEnd();
        }
    }
}