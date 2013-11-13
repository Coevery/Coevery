using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Entities.ViewModels {
    public class EntityDetailViewModel : EditTypeViewModel {
        public int Id { get; set; }
        public bool HasPublished { get; set; }
    }
}