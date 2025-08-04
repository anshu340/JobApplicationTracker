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

    // GET: api/Jobs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobsDataModel>>> GetAllJobs()
    {
        try
        {
            var jobs = await _jobsRepository.GetAllJobsAsync();
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // GET: api/Jobs/getjobsbyid?id=123
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

            return Ok(job);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // ✅ NEW ENDPOINT: GET: api/Jobs/getjobsbycompanyid?companyId=123
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
                return Ok(new List<JobsDataModel>()); // Return empty list instead of 404
            }

            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // POST: api/Jobs
    [HttpPost("submitjobs")]
    public async Task<ActionResult> CreateJob([FromBody] JobsDataModel jobDto)
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
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // PUT: api/Jobs/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateJob(int id, [FromBody] JobsDataModel jobDto)
    {
        try
        {
            if (jobDto == null || id != jobDto.JobId)
            {
                return BadRequest("Invalid job data");
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

    // DELETE: api/Jobs/{id}
    [HttpDelete("{id}")]
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