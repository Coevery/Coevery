using System;
using System.IO;
using System.Net;
using System.Text;
using Orchard;
using Orchard.FileSystems.VirtualPath;
using Orchard.Localization;
using Piedone.Combinator.Models;

namespace Piedone.Combinator.Services
{
    public class ResourceFileService : IResourceFileService
    {
        private readonly IVirtualPathProvider _virtualPathProvider;

        public Localizer T { get; set; }
        
        public ResourceFileService(
            IVirtualPathProvider virtualPathProvider)
        {
            _virtualPathProvider = virtualPathProvider;

            T = NullLocalizer.Instance;
        }


        public string GetLocalResourceContent(CombinatorResource resource)
        {
            var relativeVirtualPath = resource.RelativeVirtualPath;

            // Maybe TryFileExists would be better?
            if (!_virtualPathProvider.FileExists(relativeVirtualPath)) throw new OrchardException(T("Local resource file not found under {0}", relativeVirtualPath));

            string content;
            using (var stream = _virtualPathProvider.OpenFile(relativeVirtualPath))
            {
                content = new StreamReader(stream).ReadToEnd();
            }

            return content;
        }

        public string GetRemoteResourceContent(CombinatorResource resource)
        {
            using (var wc = new WebClient())
            {
                var byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                var content = new UTF8Encoding(false).GetString(wc.DownloadData(resource.AbsoluteUrl));
                if (content.StartsWith(byteOrderMarkUtf8)) // Stripping "?"s from the beginning of css commments "/*"
                {
                    content = content.Remove(0, byteOrderMarkUtf8.Length);
                }
                return content; 
            }
        }

        public string GetImageBase64Data(Uri imageUrl, int maxSizeKB)
        {
            // Since these are public urls referenced in stylesheets, there's no simple way to tell their local path.
            // That's why all images are downloaded with WebClient.
            using (var wc = new WebClient())
            {
                var imageData = wc.DownloadData(imageUrl);
                if (imageData.Length / 1024 > maxSizeKB) return "";
                return Convert.ToBase64String(imageData);
            }
        }
    }
}