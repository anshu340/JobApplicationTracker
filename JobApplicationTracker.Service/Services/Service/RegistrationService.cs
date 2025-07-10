
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
    ICompaniesRepository companiesRepository)
    : IRegistrationService
{
    
    public async Task<ResponseDto> RegisterUserAsync(RegisterDto request)
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

        if (request.CompanyDto != null)
        {
            // call the company service and register the company
            var createdCompanyId = await companiesRepository.CreateCompanyAsync(request.CompanyDto);
            
            // after registration populate the user table and reference the companyId as foreign key
            var companyUser = new UsersDataModel()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = request.Password,
                CompanyId = createdCompanyId,
                Email = request.Email,
                UserType = (int) UserTypes.Recruiter
            };
            
            int createdUserId = await userRepository.CreateUserAsync(companyUser);
            if (createdUserId < 1)
            {
                return new ResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Registration failed. Try again later.!"
                };
            }
        
            return new ResponseDto()
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Registered successfully."
            };
        }
        
        // if the company Dto is found null, then create a normal user i.e. jobseeker
        // request the userService and create a new user with companyId as null
        var newUser = new UsersDataModel()
        {
            Email = request.Email,
            FirstName =    request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            Location = request.Location,
            UserType = (int) UserTypes.JobSeeker,
            CreatedAt = request.CreateDateTime ?? DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        int userId = await userRepository.CreateUserAsync(newUser);

        if (userId < 1)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Registration failed. Try again later.!"
            };
        }
        
        return new ResponseDto()
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Registered successfully."
        };
    }
}