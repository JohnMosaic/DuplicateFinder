namespace DuplicateFinder.Model
{
    public class DupFileInfo
    {
        public DupFileInfo() { }

        public bool IsDel { get; set; } = false;

        public int SN { get; set; } = 0;

        public string MD5 { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Size { get; set; }

        public string Path { get; set; }
    }
}
