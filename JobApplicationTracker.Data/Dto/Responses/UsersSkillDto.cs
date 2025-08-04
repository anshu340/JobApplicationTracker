using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dtos.Responses;

public class UsersSkillDto
{
    public int UserSkillId { get; set; }        // Primary key (if available)
    public int JobSeekerId { get; set; }        // Foreign key to user
    public int SkillId { get; set; }            // Foreign key to SkillsDataModel

    // Optional: useful for display
    public string SkillName { get; set; } = null!;
    public string Category { get; set; } = null!;
}
