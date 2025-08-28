using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Data.Dto.Requests
{
    public class RejectApplicationRequest
    {
        [StringLength(500, ErrorMessage = "Rejection reason cannot exceed 500 characters.")]
        public string? RejectionReason { get; set; }
    }
}