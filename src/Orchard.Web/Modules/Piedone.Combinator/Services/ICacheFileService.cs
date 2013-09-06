using System.Collections.Generic;
using Orchard;
using Piedone.Combinator.Models;

namespace Piedone.Combinator.Services
{
    public interface ICacheFileService : IDependency
    {
        void Save(int hashCode, CombinatorResource resource);
        IList<CombinatorResource> GetCombinedResources(int hashCode);
        bool Exists(int hashCode);
        int GetCount();
        //void Delete(int hashCode);
        void Empty();
    }
}
