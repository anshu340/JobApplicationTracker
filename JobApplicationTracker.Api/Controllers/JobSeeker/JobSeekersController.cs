using JobApplicationTracker.Api.ApiResponses;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobSeeker;

[Route("api/jobSeekers")]
public class 
    JobsSeekersController(IJobSeekersRepository jobSeekerService, 
        IRegistrationService registrationService) : ControllerBase
{
    [HttpGet]
    [Route("/getalljobSeekers")]
    public async Task<IActionResult> GetAllJobSeekers()
    {
        var jobSeeker = await jobSeekerService.GetAllJobSeekersAsync();
        return Ok(jobSeeker);
    }

    [HttpGet]
    [Route("/getjobSeekerbyid")]
    public async Task<IActionResult> GetJobSeekerById(int id)
    {
        var jobSeekerr = await jobSeekerService.GetJobSeekersByIdAsync(id);
        if (jobSeekerr == null)
        {
            return NotFound();
        }
        return Ok(jobSeekerr);
    }

    [HttpPost]
    [Route("/addjobseeker")]
    public async Task<IActionResult> AddJobSeeker([FromBody] RegisterJobSeekerRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request == null)
        {
            return BadRequest("Request body cannot be empty.");
        }
        
        var response = await registrationService.RegisterJobSeekerAsync(request);
        if (response.IsSuccess)
        {
            return Created(
                string.Empty,
                new ApiResponse{
                    StatusCode = response.StatusCode,
                    Message = response.Message,
                    IsSuccess = response.IsSuccess}
                );
        }

        return BadRequest("The server is busy at the moment. Please try again later.");
    }

    [HttpPost]
    [Route("/submitjobSeeker")]
    public async Task<IActionResult> SubmitJobSeeker([FromBody] JobSeekersDataModel jobSeekersDto)
    {
        if (!ModelState.IsValid)
        {
            // ModelState.AddModelError("","");
            // throw new ValidationException("Please enter all the required fields");
            return BadRequest(ModelState);
        }
        if (jobSeekersDto == null)
        {
            return BadRequest("Request body cannot be empty.");
        }

        var response = await jobSeekerService.SubmitJobSeekersAsync(jobSeekersDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete]
    [Route("/deletejobseeker")]
    public async Task<IActionResult> DeleteJobSeeker(int id)
    {
        var response = await jobSeekerService.DeleteJobSeekersAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}