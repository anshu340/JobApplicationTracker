namespace JobApplicationTracker.Service.DTO.Response;


    public class RegistrationResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string UserType { get; set; } = null!;
        public Guid? ProfileId { get; set; } // JobSeekerId, CompanyId, or AdminId
        public string Message { get; set; } = null!;
    }
