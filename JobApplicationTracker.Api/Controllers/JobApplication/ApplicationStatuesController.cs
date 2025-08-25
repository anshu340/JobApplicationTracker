using Microsoft.AspNetCore.Mvc;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Data.Dto;

namespace JobApplicationTracker.Api.Controllers.JobApplication
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationStatusController : ControllerBase
    {
        private readonly IApplicationStatusRepository _applicationStatusRepository;

        public ApplicationStatusController(IApplicationStatusRepository applicationStatusRepository)
        {
            _applicationStatusRepository = applicationStatusRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationStatusDto>>> GetApplicationStatuses()
        {
            try
            {
                var statuses = await _applicationStatusRepository.GetAllApplicationStatusesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationStatusDto>> GetApplicationStatus(int id)
        {
            try
            {
                var status = await _applicationStatusRepository.GetApplicationStatusByIdAsync(id);

                if (status == null)
                {
                    return NotFound($"Application status with ID {id} not found.");
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<ApplicationStatusDto>> CreateApplicationStatus(ApplicationStatusDto applicationStatus)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationStatus.StatusName))
                {
                    return BadRequest("StatusName is required.");
                }

                var createdStatus = await _applicationStatusRepository.CreateApplicationStatusAsync(applicationStatus);
                return CreatedAtAction(nameof(GetApplicationStatus),
                    new { id = createdStatus.ApplicationStatusId }, createdStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationStatus(int id)
        {
            try
            {
                var deleted = await _applicationStatusRepository.DeleteApplicationStatusAsync(id);

                if (!deleted)
                {
                    return NotFound($"Application status with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}