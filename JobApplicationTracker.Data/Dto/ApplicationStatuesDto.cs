using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto
{
    public class ApplicationStatusDto
    {
        public int ApplicationStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}