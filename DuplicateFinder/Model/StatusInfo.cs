namespace DuplicateFinder.Model
{
    public class StatusInfo
    {
        public StatusInfo() { }

        public int ProgressValue { get; set; } = 0;

        public int TotalCount { get; set; } = 0;

        public int DupCount { get; set; } = 0;
    }
}
