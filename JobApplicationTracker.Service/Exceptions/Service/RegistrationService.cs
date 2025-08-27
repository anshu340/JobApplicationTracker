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
    ICompaniesRepository companiesRepository)
    : IRegistrationService
{
    public async Task<ResponseDto> RegisterUserAsync(RegisterDto request)
    {
        // Check for duplicate email
        if (await userRepository.GetUserByEmail(request.Email) != null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Email is already registered."
            };
        }

        // Check for duplicate phone number (optional field)
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            if (await userRepository.GetUserByPhone(request.PhoneNumber) != null)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "This phone number is already in use. Try logging in."
                };
            }
        }

        // ✅ UPDATED: Handle registration based on UserType
        switch (request.UserType)
        {
            case (int)UserTypes.Company:
                return await HandleCompanyRegistration(request);

            case (int)UserTypes.Staff:
                return await HandleStaffRegistration(request);

            case (int)UserTypes.User:
                return await HandleUserRegistration(request);

            case (int)UserTypes.Admin:
                return await HandleAdminRegistration(request);

            default:
                return new ResponseDto
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid user type specified."
                };
        }
    }

    private async Task<ResponseDto> HandleCompanyRegistration(RegisterDto request)
    {
        if (request.CompanyDto == null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Company information is required for company registration."
            };
        }

        var createdCompanyId = await companiesRepository.CreateCompanyAsync(request.CompanyDto);

        var companyUser = new UsersDataModel
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            CompanyId = createdCompanyId,
            Email = request.Email,
            UserType = (int)UserTypes.Company,
            Location = request.Location,
            CreatedAt = request.CreateDateTime ?? DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var createdUser = await userRepository.SubmitUsersAsync(companyUser);

        if (createdUser.Id < 1)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Company registration failed. Try again later.!"
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Company registered successfully.",
            Id = createdUser.Id
        };
    }
    private async Task<ResponseDto> HandleStaffRegistration(RegisterDto request)
    {
        // ✅ Validate CompanyId is provided
        if (!request.CompanyId.HasValue || request.CompanyId.Value <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Company ID is required for staff registration."
            };
        }

        // ✅ IMPORTANT: Verify that the company exists using your existing method
        if (!await companiesRepository.CompanyExistsAsync(request.CompanyId.Value))
        {
            return new ResponseDto
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"Company with ID {request.CompanyId.Value} does not exist. Please provide a valid company ID."
            };
        }

        // ✅ Create staff user - company is verified to exist
        var staffUser = new UsersDataModel
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            CompanyId = request.CompanyId.Value,
            Email = request.Email,
            UserType = (int)UserTypes.Staff,
            Location = request.Location,
            CreatedAt = request.CreateDateTime ?? DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var createdUser = await userRepository.SubmitUsersAsync(staffUser);

        return new ResponseDto
        {
            IsSuccess = createdUser.Id > 0,
            StatusCode = createdUser.Id > 0
                ? StatusCodes.Status200OK
                : StatusCodes.Status500InternalServerError,
            Message = createdUser.Id > 0
                ? "Staff registered successfully."
                : "Staff registration failed. Try again later!",
            Id = createdUser.Id  // ✅ Also return the created user ID
        };
    }
    private async Task<ResponseDto> HandleUserRegistration(RegisterDto request)
    {
        // Handle regular user (job seeker) registration
        var newUser = new UsersDataModel
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            Location = request.Location,
            UserType = (int)UserTypes.User,
            CreatedAt = request.CreateDateTime ?? DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var user = await userRepository.SubmitUsersAsync(newUser);

        return new ResponseDto
        {
            IsSuccess = user.Id > 0,
            StatusCode = user.Id > 0
                ? StatusCodes.Status200OK
                : StatusCodes.Status500InternalServerError,
            Message = user.Id > 0
                ? "User registered successfully."
                : "User registration failed. Try again later.!",
            Id = user.Id
        };
    }

    private async Task<ResponseDto> HandleAdminRegistration(RegisterDto request)
    {
        // ✅ OPTIONAL: Handle Admin registration (you might want to restrict this)
        var adminUser = new UsersDataModel
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            Location = request.Location,
            UserType = (int)UserTypes.Admin,
            CreatedAt = request.CreateDateTime ?? DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var user = await userRepository.SubmitUsersAsync(adminUser);

        return new ResponseDto
        {
            IsSuccess = user.Id > 0,
            StatusCode = user.Id > 0
                ? StatusCodes.Status200OK
                : StatusCodes.Status500InternalServerError,
            Message = user.Id > 0
                ? "Admin registered successfully."
                : "Admin registration failed. Try again later.!"
        };
    }
}