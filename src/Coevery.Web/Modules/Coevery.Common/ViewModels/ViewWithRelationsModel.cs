using System.Collections.Generic;

namespace Coevery.Common.ViewModels {
    public class ViewWithRelationsModel {
        public dynamic Shape { get; set; }
        public IDictionary<int, string> Relations { get; set; }
    }
}