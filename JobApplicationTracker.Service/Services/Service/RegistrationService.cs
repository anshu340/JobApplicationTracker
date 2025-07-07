
using JobApplicationTracker.Api.Enums;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
using JobApplicationTracker.Service.Exceptions;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JobApplicationTracker.Service.Services.Service;

public class RegistrationService(
    IUserRepository userRepository,
    IJobSeekersRepository jobSeekerRepository,
    IPasswordHasherService passwordHasherService)
    : IRegistrationService
{
    public async Task<ResponseDto> RegisterJobSeekerAsync(RegisterJobSeekerRequestDto request)
    {
        // the email should always be unique
        if (await userRepository.GetUserByEmail(request.Email) != null)
        {
            throw new DuplicateEmailException("Email is already registered.");
        }

        // user might not enter the phone number while registering
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            if (await userRepository.GetUserByPhone(request.PhoneNumber) != null)
            {
                throw new DuplicatePhoneNumberException("This phone number is already in use. Try logging in.");
            }
        }

        var passwordHash = passwordHasherService.HashPassword(request.Password);
     
        var newUser = new UsersDataModel
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            UserType = UserTypes.JOBSEEKER.ToString()
        };
        
        int userId = await userRepository.CreateUserAsync(newUser);
        Console.WriteLine(userId.ToString());
        if (userId < 1)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Registration failed. Try again later.!"
            };
        }

        // create a JobSeeker
        var jobSeeker = new JobSeekersDataModel{
            UserId = userId, // Link to the created user
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            ResumeUrl = request.ResumeUrl,
            PortfolioUrl = request.PortfolioUrl,
            LinkedinProfile = request.LinkedinUrl,
            Location = request.Location,
            Headline = request.Headline,
            Bio = request.Bio,
            DateOfBirth = request.DateOfBirth,
            PreferredJobTypes = request?.PreferredJobTypes?.ToString(),
            PreferredExperienceLevels = request?.PreferredExperienceLevels?.ToString()
        };
        
      
        await jobSeekerRepository.CreateJobSeekersAsync(jobSeeker);
        
        return new ResponseDto()
        {
           IsSuccess = true,
           StatusCode = StatusCodes.Status200OK,
           Message = "User successfully registered!"
        };
    }
}