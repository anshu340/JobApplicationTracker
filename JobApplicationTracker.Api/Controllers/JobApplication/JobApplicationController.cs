using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobApplication;

[Route("api/jobapplication")]
[ApiController]
public class JobsApplicationController(IJobApplicationRepository jobApplicationService) : ControllerBase
{
    [HttpGet]
    [Route("/getalljobapplication")]
    public async Task<IActionResult> GetAllJobs()
    {
        try
        {
            var jobApp = await jobApplicationService.GetAllJobApplicationAsync();
            return Ok(jobApp);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllJobs error: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while retrieving job applications.", error = ex.Message });
        }
    }

    [HttpGet]
    [Route("/getjobapplicationbyid")]
    public async Task<IActionResult> GetJobApplicationById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Valid application ID is required." });
        }

        try
        {
            var jobApp = await jobApplicationService.GetJobApplicationByIdAsync(id);
            if (jobApp == null)
            {
                return NotFound(new { message = $"Job application with ID {id} not found." });
            }
            return Ok(jobApp);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetJobApplicationById error: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while retrieving the job application.", error = ex.Message });
        }
    }

    [HttpPost]
    [Route("/submitjobapplication")]
    public async Task<IActionResult> SubmitJobApplication([FromBody] ApplicationsDataModel jobApplicationDto)
    {
        if (jobApplicationDto == null)
        {
            return BadRequest(new { message = "Job application data is required." });
        }

        // Basic validation
        if (jobApplicationDto.UserId <= 0)
        {
            return BadRequest(new { message = "Valid UserId is required." });
        }

        if (jobApplicationDto.JobId <= 0)
        {
            return BadRequest(new { message = "Valid JobId is required." });
        }

        // Validate ApplicationStatus - assuming valid status IDs are 1, 2, 3, etc.
        if (jobApplicationDto.ApplicationStatus <= 0)
        {
            // Set default to Applied (assuming 1 = Applied)
            jobApplicationDto.ApplicationStatus = 1;
            Console.WriteLine("ApplicationStatus not provided, defaulting to Applied (1)");
        }
        // You might want to add additional validation here based on your ApplicationStatus table
        // For example, check if the status ID exists in the database

        // Log the incoming request for debugging
        Console.WriteLine($"Controller received: UserId={jobApplicationDto.UserId}, JobId={jobApplicationDto.JobId}, ApplicationStatus={jobApplicationDto.ApplicationStatus}");

        try
        {
            var response = await jobApplicationService.SubmitJobApplicationAsync(jobApplicationDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                // Return more specific error codes based on the error message
                if (response.Message.Contains("does not exist"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SubmitJobApplication error: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while submitting the job application.",
                error = ex.Message
            });
        }
    }

    [HttpDelete]
    [Route("/deletejobapplication")]
    public async Task<IActionResult> DeleteJobApplication(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Valid application ID is required." });
        }

        try
        {
            var response = await jobApplicationService.DeleteJobApplicationAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteJobApplication error: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An error occurred while deleting the job application.",
                error = ex.Message
            });
        }
    }
}