using JobApplicationTracker.Data.DataModels;
using System.IO;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
using JobApplicationTracker.Service.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;



namespace JobApplicationTracker.Api.Controllers.User;

[ApiController]
//[Authorize]
[Route("/")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IRegistrationService _registrationService;
    private readonly IDatabaseConnectionService _dbConnectionService;
    private readonly IWebHostEnvironment _env;

    // Proper constructor for dependency injection
    public UsersController(
      IUserRepository userRepository,
      IPasswordHasherService passwordHasher,
      IRegistrationService registrationService,
      IDatabaseConnectionService dbConnectionService,
      IWebHostEnvironment env)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _registrationService = registrationService;
        _dbConnectionService = dbConnectionService;
        _env = env;
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
    [AllowAnonymous]
    public async Task<IActionResult> SubmitUsersAsync(UsersDataModel usersDto)
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


    [HttpPost("uploadProfilePicture/{userId:int}")]
    public async Task<IActionResult> UploadProfilePicture([FromRoute] int userId, [FromForm] UploadProfileDto uploadProfileDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string? imageUrl = null;

        if (uploadProfileDto.ProfileImage != null && uploadProfileDto.ProfileImage.Length > 0)
        {
            var permittedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!permittedTypes.Contains(uploadProfileDto.ProfileImage.ContentType))
                return BadRequest("Invalid image file type.");

            var ext = Path.GetExtension(uploadProfileDto.ProfileImage.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var saveFolder = Path.Combine(_env.WebRootPath, "images", "profiles");
            Directory.CreateDirectory(saveFolder);
            var savePath = Path.Combine(saveFolder, fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await uploadProfileDto.ProfileImage.CopyToAsync(stream);
            }

            imageUrl = $"/images/profiles/{fileName}";
        }

        // ✅ Use repository method to update DB
        var result = await _userRepository.UpdateUserProfilePictureAsync(userId, imageUrl, uploadProfileDto.Bio);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }





}