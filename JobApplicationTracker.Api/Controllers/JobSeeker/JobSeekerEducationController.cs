using JobApplicationTracker.Dto; // Add this for DTO
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobSeeker
{
    [Route("api/jobseekereducation")]
    [ApiController]
    public class JobSeekerEducationController : ControllerBase
    {
        private readonly IJobSeekersEducationRepository _jobSeekersEducationRepository;

        public JobSeekerEducationController(IJobSeekersEducationRepository jobSeekersEducationRepository)
        {
            _jobSeekersEducationRepository = jobSeekersEducationRepository;
        }

        //  Get all education records
        [HttpGet("getalljobseekereducation")]
        public async Task<IActionResult> GetAllJobSeekersEducation()
        {
            var jobEduu = await _jobSeekersEducationRepository.GetAllJobSeekerEducationAsync();
            return Ok(jobEduu);
        }

        //  Get education by EducationId (existing)
        [HttpGet("getjobseekereducation/{id}")]
        public async Task<IActionResult> GetJobSeekerEducationsById([FromRoute] int id)
        {
            var jobSeekerEdu = await _jobSeekersEducationRepository.GetJobSeekerEducationByIdAsync(id);
            return jobSeekerEdu == null ? NotFound() : Ok(jobSeekerEdu);
        }

        //  Submit new education
        [HttpPost("submitjobseekereducation")]
        public async Task<IActionResult> SubmitJobSeekerEducation([FromBody] JobSeekerEducationDto jobSeekerEducationDto) // Changed to DTO
        {
            if (jobSeekerEducationDto == null)
                return BadRequest("Request body cannot be null");

            var response = await _jobSeekersEducationRepository.SubmitJobSeekerEducationAsync(jobSeekerEducationDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        // FOR Delete education
        [HttpDelete("deletejobseekereducation/{id}")] // Fixed route parameter
        public async Task<IActionResult> DeleteJobSeekerEducation([FromRoute] int id) // Added FromRoute
        {
            var response = await _jobSeekersEducationRepository.DeleteJobSeekerEducationAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}