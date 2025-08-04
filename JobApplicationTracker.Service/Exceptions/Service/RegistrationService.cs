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



        if (request.CompanyDto != null)
        {
            var createdCompanyId = await companiesRepository.CreateCompanyAsync(request.CompanyDto);
            //add login if company created

            var companyUser = new UsersDataModel
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = request.Password,
                CompanyId = createdCompanyId,
                Email = request.Email,
                UserType = (int)UserTypes.Company,
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
                    Message = "Registration failed. Try again later.!"
                };
            }

            return new ResponseDto
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Registered successfully."
            };
        }
        else
        {

            // Handle job seeker (no company)
            var newUser = new UsersDataModel
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = request.Password,
                Location = request.Location,
                UserType = (int)UserTypes.JobSeeker,
                CreatedAt = request.CreateDateTime ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var user = await userRepository.SubmitUsersAsync(newUser);
            if (user.Id < 1)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Registration failed. Try again later.!"
                };
            }

            return new ResponseDto
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Registered successfully."
            };
        }
    }
}
