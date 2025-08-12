using JobApplicationTracker.Dto; // Add this for DTO
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.Users
{
    [Route("api/userseducation")]
    [ApiController]
    public class UsersEducationController : ControllerBase
    {
        private readonly IUsersEducationRepository _usersEducationRepository;

        public UsersEducationController(IUsersEducationRepository usersEducationRepository)
        {
            _usersEducationRepository = usersEducationRepository;
        }

        // Get all education records
        [HttpGet("getalluserseducation")]
        public async Task<IActionResult> GetAllUsersEducation()
        {
            var educationList = await _usersEducationRepository.GetAllUsersEducationAsync();
            return Ok(educationList);
        }

        // Get education by EducationId
        [HttpGet("getuserseducation/{id}")]
        public async Task<IActionResult> GetUsersEducationById([FromRoute] int id)
        {
            var education = await _usersEducationRepository.GetUsersEducationByIdAsync(id);
            return education == null ? NotFound() : Ok(education);
        }

        // Submit new education
        [HttpPost("submituserseducation")]
        public async Task<IActionResult> SubmitUsersEducation([FromBody] UsersEducationDto usersEducationDto)
        {
            if (usersEducationDto == null)
                return BadRequest("Request body cannot be null");

            var response = await _usersEducationRepository.SubmitUsersEducationAsync(usersEducationDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        // Delete education
        [HttpDelete("deleteuserseducation/{id}")]
        public async Task<IActionResult> DeleteUsersEducation([FromRoute] int id)
        {
            var response = await _usersEducationRepository.DeleteUsersEducationAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
