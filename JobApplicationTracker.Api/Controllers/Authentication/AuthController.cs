
using JobApplicationTracker.Data.Dto.AuthDto;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _usersService;
        private readonly ICookieService _cookieService;
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IUserRepository usersService, ICookieService cookieService, 
            IAuthenticationService authenticationService)
        {
            _usersService = usersService;
            _cookieService = cookieService;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(LoginDto credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (credentials == null)
            {
                return BadRequest(ModelState);
            }

            var user = await _usersService.GetUserForLoginAsync(credentials.Email);

            // check for user existence
            if (user == null)
            {
                return BadRequest(new ResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "User does not exist. Please sign up to login."
                });
            }

            // match the credentials
            if (!BCrypt.Net.BCrypt.Verify(credentials.Password, user.PasswordHash))
            {
                return BadRequest(new ResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Incorrect Password. Please try again later."
                });
            }

            var jwtToken = _authenticationService.GenerateJwtToken(user);
            // _cookieService.AppendCookies(Response, jwtToken);

            var response = new ResponseDto()
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Login Successful."
            };

            return Ok(new
            {
                response,
                jwtToken,
                user.UserType,
                firstName = user.FirstName,
                lastName = user.LastName
            });

        }
    }
}
