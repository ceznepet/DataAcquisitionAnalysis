
namespace Common.Models.Setup
{
    public class LoadDataDBSetup
    {
        public string DatabaseLocation { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseCollection { get; set; }
        public bool Profinet { get; set; }
        public string SaveFolderLocation { get; set; }
        public string Filename { get; set; }
        public bool SortData { get; set; }
        public bool SortByProduct { get; set; }
        public bool ToMatFile { get; set; }
    }
}
