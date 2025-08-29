using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.Requests;
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

    // NEW ENDPOINT: Get job applications by user ID
    [HttpGet]
    [Route("/getjobapplicationsbyuserid")]
    public async Task<IActionResult> GetJobApplicationsByUserId(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(new { message = "Valid user ID is required." });
        }

        try
        {
            var applications = await jobApplicationService.GetJobApplicationsByUserIdAsync(userId);

            if (!applications.Any())
            {
                return Ok(new
                {
                    message = $"No job applications found for user ID {userId}.",
                    data = applications
                });
            }

            return Ok(new
            {
                message = $"Found {applications.Count()} job applications for user ID {userId}.",
                data = applications
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetJobApplicationsByUserId error: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving job applications for the user.",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("/getapplicationsbycompanyid")]
    public async Task<IActionResult> GetApplicationsByCompanyId(int companyId)
    {
        if (companyId <= 0)
        {
            return BadRequest(new { message = "Valid company ID is required." });
        }

        try
        {
            var applications = await jobApplicationService.GetApplicationsByCompanyIdAsync(companyId);

            if (!applications.Any())
            {
                return Ok(new
                {
                    message = $"No applications found for company ID {companyId}.",
                    data = applications
                });
            }

            return Ok(new
            {
                message = $"Found {applications.Count()} applications for company ID {companyId}.",
                data = applications
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetApplicationsByCompanyId error: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving applications for the company.",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("/submitjobapplication")]
    public async Task<IActionResult> SubmitJobApplication([FromBody] JobApplicationsDataModel jobApplicationDto)
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
        if (jobApplicationDto.ApplicationStatus <= 0)
        {
            jobApplicationDto.ApplicationStatus = 1;
            Console.WriteLine("ApplicationStatus not provided, defaulting to Applied (1)");
        }

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


    [HttpPost]
    [Route("/acceptjobapplication")]
    public async Task<IActionResult> AcceptJobApplication(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Valid application ID is required." });
        }

        try
        {
            var response = await jobApplicationService.AcceptJobApplicationAsync(id);

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
            Console.WriteLine($"AcceptJobApplication error: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An error occurred while accepting the job application.",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [Route("/rejectjobapplication")]
    public async Task<IActionResult> RejectJobApplication(int id, [FromBody] RejectApplicationRequest? request = null)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Valid application ID is required." });
        }

        try
        {
            var rejectionReason = request?.RejectionReason;
            var response = await jobApplicationService.RejectJobApplicationAsync(id, rejectionReason);

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
            Console.WriteLine($"RejectJobApplication error: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An error occurred while rejecting the job application.",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("/getacceptedjobapplicationsbyuserid")]
    public async Task<IActionResult> GetAcceptedJobApplicationsByUserId(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(new { message = "Valid user ID is required." });
        }

        try
        {
            var applications = await jobApplicationService.GetAcceptedJobApplicationsByUserIdAsync(userId);

            if (!applications.Any())
            {
                return Ok(new
                {
                    message = $"No accepted job applications found for user ID {userId}.",
                    data = applications
                });
            }

            return Ok(new
            {
                message = $"Found {applications.Count()} accepted job applications for user ID {userId}.",
                data = applications
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAcceptedJobApplicationsByUserId error: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving accepted job applications for the user.",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("/getrejectedjobapplicationsbyuserid")]
    public async Task<IActionResult> GetRejectedJobApplicationsByUserId(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(new { message = "Valid user ID is required." });
        }

        try
        {
            var applications = await jobApplicationService.GetRejectedJobApplicationsByUserIdAsync(userId);

            if (!applications.Any())
            {
                return Ok(new
                {
                    message = $"No rejected job applications found for user ID {userId}.",
                    data = applications
                });
            }

            return Ok(new
            {
                message = $"Found {applications.Count()} rejected job applications for user ID {userId}.",
                data = applications
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetRejectedJobApplicationsByUserId error: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving rejected job applications for the user.",
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