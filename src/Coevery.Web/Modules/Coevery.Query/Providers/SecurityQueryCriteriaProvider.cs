namespace Coevery.Query.Providers {
    public class SecurityQueryCriteriaProvider : IQueryCriteriaProvider {
        public SecurityQueryCriteriaProvider(ICoeveryServices services) {
            Services = services;
        }

        public ICoeveryServices Services { get; set; }

        public void Apply(QueryContext context) {
            var user = Services.WorkContext.CurrentUser;
            string typeName = context.ContentTypeName;

            if (typeName != "Lead") {
                return;
            }
        }
    }
}