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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobsDataModel>>> GetAllJobs()
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
                return Ok(new List<JobsDataModel>()); // empty list
            }

            var updatedJobs = SetJobsStatus(jobs);
            return Ok(updatedJobs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // POST: api/Jobs/submitjobs
    [HttpPost("submitjobs")]
    public async Task<ActionResult> SubmitJob([FromBody] JobsDataModel jobDto)
    {
        try
        {
            if (jobDto == null)
            {
                return BadRequest("Job data is required");
            }

            var result = await _jobsRepository.SubmitJobAsync(jobDto);

            if (result.IsSuccess)
            {
                // ✅ Return the ResponseDto with success message and ID
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // DELETE: api/Jobs/delete/{id}
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