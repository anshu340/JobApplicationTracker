namespace JobApplicationTracker.Data.Dtos
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public int UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool? IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? LinkUrl { get; set; }
    }
}
