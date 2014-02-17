using System.Linq;
using Coevery.Common.Extensions;
using Coevery.Entities.Events;
using Coevery.Projections.Models;
using Coevery.Data;
using Coevery.ContentManagement;
using Coevery.Projections.ViewModels;

namespace Coevery.Projections.Handlers
{
    public class ListViewFieldHandler : IFieldEvents
    {
        private readonly IRepository<ListViewPart> _listViewRepository;
        private readonly IRepository<PropertyRecord> _propertyRepository;
        private readonly IContentManager _contentManager;

        public ListViewFieldHandler(IRepository<ListViewPart> listViewRepository, 
            IRepository<PropertyRecord> propertyRepository, 
            ICoeveryServices services,
            IContentManager contentManager)
        {
            _listViewRepository = listViewRepository;
            _propertyRepository = propertyRepository;
            Services = services;
            _contentManager = contentManager;
        }

        public ICoeveryServices Services { get; private set; }

        public void OnCreated(string entityName, string fieldName, bool isInLayout)
        { 
        }

        public void OnDeleting(string entityName, string fieldName)
        {
            var parts = Services.ContentManager.Query<ListViewPart, ListViewPartRecord>()
                .Where(r => r.ItemContentType == entityName).List();

            foreach (var part in parts)
            {
                var projectionPart = part.As<ProjectionPart>().Record;
                var layout = projectionPart.LayoutRecord;
                string type = string.Format("{0}.{1}.", entityName.ToPartName(), fieldName);
                var deletedFields = layout.Properties.Where(x => x.Type == type).ToList();
                deletedFields.ForEach(record => layout.Properties.Remove(record));
            }

            //var projections = _contentManager.Query<ListViewPart, ListViewPartRecord>().Where(x => x.ItemContentType == entityName).List();
            //foreach (var projection in projections)
            //{
            //    string type = string.Format("{0}.{1}.", entityName.ToPartName(), fieldName);
            //    var filters = projection.FilterGroupRecord.Filters.Where(x => x.Type == type).ToList();
            //    filters.ForEach(record => entityFilterRecord.FilterGroupRecord.Filters.Remove(record));
            //    if (entityFilterRecord.FilterGroupRecord.Filters.Count == 0)
            //    {
            //        _entityFilterRepository.Delete(entityFilterRecord);
            //    }
            //    else
            //    {
            //        _entityFilterRepository.Update(entityFilterRecord);
            //    }
            //}
        }
    }
}