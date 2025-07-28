using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.User;

[ApiController]
[Route("/")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IRegistrationService _registrationService;

    // Proper constructor for dependency injection
    public UsersController(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasher,
        IRegistrationService registrationService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _registrationService = registrationService;
    }

    [HttpGet("getallusers")]
    public async Task<IActionResult> GetAllUsers(int companyId)
    {
        var users = await _userRepository.GetAllUsersAsync(companyId);
        return Ok(users);
    }

    [HttpGet("getusersbyid")]
    public async Task<IActionResult> GetUserById([FromQuery] int id)
    {
        var user = await _userRepository.GetUsersByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("submitusers")]
    public async Task<IActionResult> SubmitUsers([FromBody] UsersDataModel usersDto)
    {
        if (usersDto == null)
            return BadRequest();

        usersDto.PasswordHash = _passwordHasher.HashPassword(usersDto.PasswordHash);

        var response = await _userRepository.SubmitUsersAsync(usersDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("deleteuser")]
    public async Task<IActionResult> DeleteUser([FromQuery] int id)
    {
        var response = await _userRepository.DeleteUsersAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        request.Password = _passwordHasher.HashPassword(request.Password);

        var response = await _registrationService.RegisterUserAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("profile/{userId}")]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(int userId)
    {
        var profile = await _userRepository.GetUserProfileAsync(userId);

        if (profile == null)
            return NotFound("User not found");

        return Ok(profile);
    }
}
