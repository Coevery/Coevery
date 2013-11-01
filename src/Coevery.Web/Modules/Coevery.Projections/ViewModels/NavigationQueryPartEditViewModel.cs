using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Coevery.Projections.Models;

namespace Coevery.Projections.ViewModels {
    public class NavigationQueryPartEditViewModel {

        [Required, Range(0, int.MaxValue)]
        public int Items { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Skip { get; set; }

        [Required(ErrorMessage = "You must select a Query")]
        public string QueryRecordId { get; set; }

        public IEnumerable<QueryPart> Queries { get; set; }
    }
}