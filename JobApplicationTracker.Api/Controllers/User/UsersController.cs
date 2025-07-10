using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.User;

[Route("api/users")]
public class UsersController(
    IUserRepository userService,
    IPasswordHasherService passwordHasher,
    IRegistrationService registrationService
    ) : ControllerBase
{
    [HttpGet]
    [Route("/getallusers")]
    public async Task<IActionResult> GetAllJobTypes()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet]
    [Route("/getusersbyid")]
    public async Task<IActionResult> GetUsersById(int id)
    {
        var user = await userService.GetUsersByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    [Route("/submitusers")]
    public async Task<IActionResult> SubmitUsers([FromBody] UsersDataModel usersDto)
    {
        if (usersDto == null)
        {
            return BadRequest();
        }

        usersDto.PasswordHash = passwordHasher.HashPassword(usersDto.PasswordHash);

        var response = await userService.SubmitUsersAsync(usersDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete]
    [Route("/deleteuser")]
    public async Task<IActionResult> DeleteJobType(int id)
    {
        var response = await userService.DeleteUsersAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
    
    
    /*
     * Route for registering a new user (jobseeker) or a company with it's recuriter
     */
    [HttpPost]
    [Route("/register-user")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
            
        request.Password = passwordHasher.HashPassword(request.Password);
        var response = await registrationService.RegisterUserAsync(request);  
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
    
}