using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;
using Orchard.WebApi.Common;

namespace Coevery.Metadata.Controllers
{

    public class MetadataController : ApiController
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public MetadataController(IContentDefinitionService contentDefinitionService, 
            IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
        }

        // GET api/metadata/metadata
        public IEnumerable<EditTypeViewModelDto> Get()
        {
            List<EditTypeViewModelDto> metadataList = new List<EditTypeViewModelDto>();
            var metadataTypes = _contentDefinitionService.GetTypes().ToList();
            metadataTypes.ForEach(c => {
                metadataList.Add(new EditTypeViewModelDto(){ DisplayName = c.DisplayName,Name = c.Name});
            });
            return metadataList;
        }

        // GET api/leads/lead/5
        public virtual EditTypeViewModelDto GetContent(string name) {
            var metadataType = _contentDefinitionService.GetType(name);
            return new EditTypeViewModelDto() { DisplayName = metadataType.DisplayName,Name = metadataType.Name};
        }

    }
}