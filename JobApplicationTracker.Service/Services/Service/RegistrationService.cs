
using JobApplicationTracker.Api.Enums;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
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
        var existingUser = await userRepository.GetUserByEmail(request.Email);
        if (existingUser != null)
        {
            throw new ApplicationException("Email is already registered.");
        }

        var passwordHash = passwordHasherService.HashPassword(request.Password);
     
        var newUser = new UsersDataModel
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            UserType = UserTypes.JOBSEEKER.ToString()
        };

        // create a job seeker
        var jobSeeker = new JobSeekersDataModel{
            UserId = newUser.UserId, // Link to the created user
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
        
        await userRepository.SubmitUsersAsync(newUser); 
        await jobSeekerRepository.SubmitJobSeekersAsync(jobSeeker);
        
        return new ResponseDto()
        {
           IsSuccess = true,
           StatusCode = StatusCodes.Status200OK,
           Message = "User successfully registered!"
        };
    }
}