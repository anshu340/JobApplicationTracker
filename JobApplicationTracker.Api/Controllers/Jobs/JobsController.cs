using Microsoft.AspNetCore.Mvc;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Data.DataModels;

namespace JobApplicationTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobsRepository _jobsRepository;

    public JobsController(IJobsRepository jobsRepository)
    {
        _jobsRepository = jobsRepository;
    }

    private JobsDataModel SetJobStatus(JobsDataModel job)
    {
        if (job.ApplicationDeadline < DateTime.UtcNow.Date)
        {
            job.Status = "I";
        }
        else
        {
            job.Status = "A";
        }
        return job;
    }

    private IEnumerable<JobsDataModel> SetJobsStatus(IEnumerable<JobsDataModel> jobs)
    {
        foreach (var job in jobs)
        {
            SetJobStatus(job);
        }
        return jobs;
    }

    // ✅ Updated: For USER-FACING pages (Home page, job search) - auto-deletes old inactive jobs + ONLY PUBLISHED JOBS
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobsDataModel>>> GetAllJobs()
    {
        try
        {
            var jobs = await _jobsRepository.GetActiveJobsForUsersAsync();
            var updatedJobs = SetJobsStatus(jobs);
            return Ok(updatedJobs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // ✅ New endpoint: For COMPANY dashboard - shows ALL jobs including inactive and unpublished ones
    [HttpGet("company/all")]
    public async Task<ActionResult<IEnumerable<JobsDataModel>>> GetAllJobsForCompany()
    {
        try
        {
            var jobs = await _jobsRepository.GetAllJobsAsync();
            var updatedJobs = SetJobsStatus(jobs);
            return Ok(updatedJobs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("getjobsbyid")]
    public async Task<ActionResult<JobsDataModel>> GetJobById([FromQuery] int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid job ID");
            }

            var job = await _jobsRepository.GetJobsByIdAsync(id);

            if (job == null)
            {
                return NotFound($"Job with ID {id} not found");
            }

            var updatedJob = SetJobStatus(job);
            return Ok(updatedJob);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // ✅ Company-specific jobs (NO auto-delete) - Shows ALL jobs including unpublished
    [HttpGet("getjobsbycompanyid")]
    public async Task<ActionResult<IEnumerable<JobsDataModel>>> GetJobsByCompanyId([FromQuery] int companyId)
    {
        try
        {
            if (companyId <= 0)
            {
                return BadRequest("Invalid company ID");
            }

            var jobs = await _jobsRepository.GetJobsByCompanyIdAsync(companyId);

            if (!jobs.Any())
            {
                return Ok(new List<JobsDataModel>());
            }

            var updatedJobs = SetJobsStatus(jobs);
            return Ok(updatedJobs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // ✅ NEW ENDPOINT: Publish a job
    [HttpPut("publish/{id}")]
    public async Task<ActionResult> PublishJob(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid job ID");
            }

            var result = await _jobsRepository.PublishJobAsync(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // ✅ NEW ENDPOINT: Unpublish a job
    [HttpPut("unpublish/{id}")]
    public async Task<ActionResult> UnpublishJob(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid job ID");
            }

            var result = await _jobsRepository.UnpublishJobAsync(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("submitjobs")]
    public async Task<ActionResult> SubmitJob([FromBody] JobsDataModel jobDto)
    {
        try
        {
            if (jobDto == null)
            {
                return BadRequest("Job data is required");
            }

            // ✅ By default, new jobs are created as unpublished (IsPublished = false)
            if (jobDto.JobId <= 0) // New job
            {
                jobDto.IsPublished = false; // Ensure new jobs are unpublished by default
            }

            var result = await _jobsRepository.SubmitJobAsync(jobDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteJob(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid job ID");
            }

            var result = await _jobsRepository.DeleteJobAsync(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}