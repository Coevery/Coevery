namespace Coevery.FormDesigner.Models {
    public class Section {
        public int SectionColumns { get; set; }
        public string SectionColumnsWidth { get; set; }
        public string SectionTitle { get; set; }
        public Row[] Rows { get; set; }
    }
}