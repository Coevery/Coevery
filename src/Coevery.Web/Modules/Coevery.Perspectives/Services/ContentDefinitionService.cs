
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coevery.Common.Extensions;
using Coevery.Perspectives.ViewModels;


namespace Coevery.Perspectives.Services
{
    public class ContentDefinitionService : IContentDefinitionService
    {
        private readonly IContentDefinitionExtension _contentDefinitionExtension;

        public ContentDefinitionService(IContentDefinitionExtension contentDefinitionExtension)
        {
            _contentDefinitionExtension = contentDefinitionExtension;
        }


        public IEnumerable<EditTypeViewModel> GetUserDefinedTypes()
        {
            return _contentDefinitionExtension.ListUserDefinedTypeDefinitions()==null?null: 
                _contentDefinitionExtension.ListUserDefinedTypeDefinitions().Select(ctd => new EditTypeViewModel(ctd)).OrderBy(m => m.DisplayName);
        }
    }
}