namespace JobApplicationTracker.Data.Dtos
{
    public class NotificationTypesDto
    {
        public int NotificationTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; }
    }
}
