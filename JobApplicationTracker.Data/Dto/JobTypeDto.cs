using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto
{
    internal class JobTypeDto
    {
        public int JobTypeId { get; set; }
        public string Name { get; set; } = null!;
    }
}
