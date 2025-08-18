namespace JobApplicationTracker.Data.DataModels
{

    public class ExperienceDataModel
    {
        public int ExperienceId { get; set; }
        public bool IsCurrentlyWorking { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public string? Description { get; set; }
        public string? JobTitle { get; set; }
        public string? Organization { get; set; }
        public string? Location { get; set; }
    }
}