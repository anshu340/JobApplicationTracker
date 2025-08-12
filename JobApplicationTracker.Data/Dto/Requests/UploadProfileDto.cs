using Microsoft.AspNetCore.Http;

namespace JobApplicationTracker.Service.DTO.Requests
{
    public class UploadProfileDto
    {

        public IFormFile ProfileImage { get; set; }
        public string Bio { get; set; }
    }
}