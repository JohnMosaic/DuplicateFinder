namespace DuplicateFinder.Utils.Common
{
    public class TypeFormatter
    {
        public string GetFileSize(long size)
        {
            if (size < 1024) return size.ToString() + "B";
            else if (size >= 1024 && size < 1024 * 1024) return (size / 1024.0).ToString(".##") + "KB";
            else if (size >= 1024 * 1024 && size < 1024 * 1024 * 1024) return ((size / 1024.0) / 1024.0).ToString(".##") + "MB";
            else return (((size / 1024.0) / 1024.0) / 1024.0).ToString(".##") + "GB";
        }
    }
}
