using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.Skills
{
    [ApiController]
    [Route("api/skills")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillsRepository skillService;

        public SkillsController(ISkillsRepository skillService)
        {
            this.skillService = skillService;
        }

        [HttpGet("getallskills")]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await skillService.GetAllSkillsAsync();
            return Ok(skills);
        }

        [HttpGet("getskillbyid")]
        public async Task<IActionResult> GetSkillsById([FromQuery] int id)
        {
            var skills = await skillService.GetSkillsByIdAsync(id);
            return skills == null ? NotFound() : Ok(skills);
        }

        // New endpoint to get skills by UserId (returns full skill objects)
        [HttpGet("getskillsbyuserid")]
        public async Task<IActionResult> GetSkillsByUserId([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var skills = await skillService.GetSkillsByUserIdAsync(userId);
            return Ok(skills);
        }

        // New endpoint to get only skill IDs by UserId
        [HttpGet("getskillsidbyuserid")]
        public async Task<IActionResult> GetSkillsIdByUserId([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var skillIds = await skillService.GetSkillsIdByUserIdAsync(userId);
            return Ok(skillIds);
        }

        [HttpPost("submitskills")]
        public async Task<IActionResult> SubmitSkills([FromBody] SkillsDataModel skillsDto)
        {
            if (skillsDto == null || string.IsNullOrWhiteSpace(skillsDto.Skill))
            {
                return BadRequest("Invalid input data.");
            }

            var response = await skillService.SubmitSkillsAsync(skillsDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("deleteskills")]
        public async Task<IActionResult> DeleteSkills([FromQuery] int id)
        {
            var response = await skillService.DeleteSkillsAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}