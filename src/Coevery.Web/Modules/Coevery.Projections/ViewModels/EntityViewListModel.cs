using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using Coevery.Localization;
using Coevery.Projections.Descriptors.Layout;

namespace Coevery.Projections.ViewModels {
    public class EntityViewListModel {
        public IEnumerable<LayoutDescriptor> Layouts { get; set; }
    }
}