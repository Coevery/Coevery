using System.Linq;
using Coevery.Entities.Events;
using Coevery.Projections.Models;
using Orchard.Data;

namespace Coevery.Projections.Handlers {
    public class FilterFieldEventsHandler : IFieldEvents {
        private readonly IRepository<EntityFilterRecord> _entityFilterRepository;

        public FilterFieldEventsHandler(IRepository<EntityFilterRecord> entityFilterRepository) {
            _entityFilterRepository = entityFilterRepository;
        }

        public void OnCreated(string entityName, string fieldName, bool isInLayout) {}

        public void OnDeleting(string entityName, string fieldName) {
            var entityFilterRecords = _entityFilterRepository.Table.Where(x => x.EntityName == entityName).ToList();
            foreach (var entityFilterRecord in entityFilterRecords) {
                string type = string.Format("{0}.{1}.", entityName, fieldName);
                var filters = entityFilterRecord.FilterGroupRecord.Filters.Where(x => x.Type == type).ToList();
                filters.ForEach(record => entityFilterRecord.FilterGroupRecord.Filters.Remove(record));
                if (entityFilterRecord.FilterGroupRecord.Filters.Count == 0) {
                    _entityFilterRepository.Delete(entityFilterRecord);
                }
                else {
                    _entityFilterRepository.Update(entityFilterRecord);
                }
            }
        }
    }
}