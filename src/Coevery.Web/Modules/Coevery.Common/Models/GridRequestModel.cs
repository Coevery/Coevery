namespace Coevery.Common.Models {
    public class GridRequestModel {
        public int Page { get; set; }
        public int Rows { get; set; }
        public bool Search { get; set; }
        public int Nd { get; set; }
        public string Sidx { get; set; }
        public string Sord { get; set; }
    }
}