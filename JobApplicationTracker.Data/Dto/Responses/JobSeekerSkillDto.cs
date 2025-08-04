using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Dto
{
    public class JobSeekerSkillDto
    {
        public int JobSeekerSkillsId { get; set; }
        public int JobSeekerId { get; set; }
        public int SkillId { get; set; }
        public int ProficiencyLevel { get; set; }
    }
}

