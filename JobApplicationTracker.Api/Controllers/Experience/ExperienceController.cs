using JobApplicationTracker.Api.ApiResponses;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.Experience
{
    [Route("api/experiences")]
    public class ExperienceController(IExperienceRepository experienceService) : ControllerBase
    {
        [HttpGet]
        [Route("/getallexperiences")]
        public async Task<IActionResult> GetAllExperiences()
        {
            try
            {
                var experiences = await experienceService.GetAllExperiencesAsync();
                return Ok(experiences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
        }

        [HttpGet]
        [Route("/getexperiencebyid")]
        public async Task<IActionResult> GetExperienceById(int id)
        {
            try
            {
                var experience = await experienceService.GetExperienceByIdAsync(id);
                if (experience == null)
                {
                    return NotFound(new { Message = "Experience not found" });
                }
                return Ok(experience);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
        }

        [HttpGet]
        [Route("/getexperiencebyuserid")]
        public async Task<IActionResult> GetExperienceByUserId([FromQuery] int userId)
        {
            try
            {
                var experiences = await experienceService.GetExperiencesByUserIdAsync(userId);
                if (experiences == null || !experiences.Any())
                    return NotFound(new { Message = "No experiences found for this user." });
                return Ok(experiences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
        }

        [HttpPost]
        [Route("/submitexperience")]
        public async Task<IActionResult> SubmitExperience([FromBody] ExperienceDataModel experienceDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                ResponseDto response;

                // Check if this is an update (has ExperienceId > 0) or create (ExperienceId = 0)
                if (experienceDto.ExperienceId > 0)
                {
                    // Update existing experience
                    response = await experienceService.UpdateExperienceAsync(experienceDto.ExperienceId, experienceDto);
                }
                else
                {
                    // Create new experience
                    response = await experienceService.SubmitExperienceAsync(experienceDto);
                }

                if (response.IsSuccess)
                    return Ok(response);

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
        }

        [HttpDelete]
        [Route("/deleteexperience")]
        public async Task<IActionResult> DeleteExperience([FromQuery] int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid experience ID" });

                var response = await experienceService.DeleteExperienceAsync(id);
                if (response is ResponseDto responseDto)
                {
                    return responseDto.IsSuccess ? Ok(responseDto) : BadRequest(responseDto);
                }
                return BadRequest("Invalid response type.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
        }
    }
}