using System;
using Orchard;
using Piedone.Combinator.Models;

namespace Piedone.Combinator.Services
{
    public interface IResourceFileService : IDependency
    {
        string GetLocalResourceContent(CombinatorResource resource);
        string GetRemoteResourceContent(CombinatorResource resource);
        string GetImageBase64Data(Uri imageUrl, int maxSizeKB);
    }
}
