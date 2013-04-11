using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace Coevery.Core.Services
{
    public interface IProjectionService:IDependency
    {
        int GetProjectionId(string entityType);
    }
}