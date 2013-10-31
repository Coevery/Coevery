using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Entities.ViewModels {
    public class EntityDetailViewModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool HasPublished { get; set; }
        public string PublishTip { get; set; }
    }
}