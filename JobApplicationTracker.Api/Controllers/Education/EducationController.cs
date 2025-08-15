using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationController(IEducationRepository educationRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllEducation()
        {
            var result = await educationRepository.GetAllEducationAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEducationById(int id)
        {
            var education = await educationRepository.GetEducationByIdAsync(id);
            if (education is null)
                return NotFound(new { Message = "Education record not found." });

            return Ok(education);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateEducation([FromBody] EducationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await educationRepository.SubmitEducationAsync(dto);
            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var response = await educationRepository.DeleteEducationAsync(id);
            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        // New endpoint: get full education details by UserId (based on JSON in Users table)
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetEducationByUserId(int userId)
        {
            var educations = await educationRepository.GetEducationByUserIdAsync(userId);
            return Ok(educations);
        }

        // New endpoint: get only education IDs by UserId (based on JSON in Users table)
        [HttpGet("user/{userId:int}/ids")]
        public async Task<IActionResult> GetEducationIdsByUserId(int userId)
        {
            var educationIds = await educationRepository.GetEducationIdsByUserIdAsync(userId);
            return Ok(educationIds);
        }
    }
}
