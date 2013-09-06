using System.Collections.Generic;

namespace Coevery.Core.ViewModels
{
    public class ViewWithRelationsModel
    {
        public dynamic Shape { get; set; }
        public IDictionary<int, string> Relations { get; set; }
    }
}