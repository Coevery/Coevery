using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace Coevery.Core.Services
{
    public interface IViewPartService:IDependency
    {
        int GetProjectionId(string entityType);
        void SetView(string entityType, int projectionId);
    }
}