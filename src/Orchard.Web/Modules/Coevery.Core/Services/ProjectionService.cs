using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Projections.Models;

namespace Coevery.Core.Services
{
    public class ProjectionService:IProjectionService
    {
        private readonly IContentManager _contentManager;

        public ProjectionService(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public int GetProjectionId(string entityType)
        {
            var projectionPart = _contentManager.Query<ProjectionPart>()
                                                .Join<TitlePartRecord>()
                                                .Where(t => t.Title == entityType)
                                                .Join<CommonPartRecord>()
                                                .OrderByDescending(t=>t.ModifiedUtc)
                                                .ForVersion(VersionOptions.Latest)
                                                .Slice(0, 1)
                                                .FirstOrDefault();

            if (projectionPart != null)
            {
                return projectionPart.Id;    
            }
            else
            {
                return -1;
            }
            
        }
    }
}