using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Requests
{
    public class UpdateStatusRequestDto
    {
        public int ApplicationId { get; set; }
        public string Action { get; set; } = string.Empty; // "accept" or "reject"
        public string? RejectionReason { get; set; }
    }
}
