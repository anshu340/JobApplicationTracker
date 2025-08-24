using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController(IExperienceRepository experienceRepository) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAllExperiences()
        {
            try
            {
                var result = await experienceRepository.GetAllExperiencesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving experiences.", Error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetExperienceById(int id)
        {
            try
            {
                var experience = await experienceRepository.GetExperienceByIdAsync(id);
                if (experience is null)
                    return NotFound(new { Message = "Experience record not found." });

                return Ok(experience);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the experience.", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExperience([FromBody] ExperienceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = MapDtoToDataModel(dto);
                var response = await experienceRepository.SubmitExperienceAsync(model);

                if (!response.IsSuccess)
                    return BadRequest(response);

                // Return appropriate status code
                if (dto.ExperienceId > 0)
                    return Ok(response);
                else
                    return Created($"api/experience/{response.Id}", response); // 201 for create
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while submitting the experience.", Error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            try
            {
                var response = await experienceRepository.DeleteExperienceAsync(id);

                if (!response.IsSuccess)
                {
                    if (response.Message.Contains("not found"))
                        return NotFound(response);

                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the experience.", Error = ex.Message });
            }
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetExperiencesByUserId(int userId)
        {
            try
            {
                var experiences = await experienceRepository.GetExperiencesByUserIdAsync(userId);
                return Ok(experiences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving user experiences.", Error = ex.Message });
            }
        }
        [HttpGet("user/{userId:int}/ids")]
        public async Task<IActionResult> GetExperienceIdsByUserId(int userId)
        {
            try
            {
                var experiences = await experienceRepository.GetExperiencesByUserIdAsync(userId);
                var experienceIds = experiences.Select(e => e.ExperienceId).ToList();
                return Ok(experienceIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving user experience IDs.", Error = ex.Message });
            }
        }

        private static ExperienceDataModel MapDtoToDataModel(ExperienceDto dto)
        {
            return new ExperienceDataModel
            {
                ExperienceId = dto.ExperienceId,
                JobTitle = dto.JobTitle,
                Organization = dto.Organization,
                Location = dto.Location,
                StartMonth = dto.StartMonth,
                StartYear = dto.StartYear,
                EndMonth = dto.EndMonth,
                EndYear = dto.EndYear,
                Description = dto.Description,
                IsCurrentlyWorking = dto.IsCurrentlyWorking
            };
        }
    }
}
