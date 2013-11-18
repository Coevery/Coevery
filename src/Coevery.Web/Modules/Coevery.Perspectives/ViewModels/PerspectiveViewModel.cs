using Coevery.Core.Common.ViewModels;

namespace Coevery.Perspectives.ViewModels {
    public class PerspectiveViewModel {
        public PerspectiveViewModel() {
            RowSortSetting = new GridRowSortSettingViewModel();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Position { get; set; }
        public GridRowSortSettingViewModel RowSortSetting { get; set; }
    }
}