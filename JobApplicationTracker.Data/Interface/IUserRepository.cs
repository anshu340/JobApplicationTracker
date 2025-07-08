
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.AuthDto;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IUserRepository
{
    Task<IEnumerable<UsersDtoResponse>> GetAllUsersAsync();
    Task<UsersDtoResponse?> GetUserByEmail(string email);
    Task<UsersDtoResponse?> GetUsersByIdAsync(int userId);
    Task<ResponseDto> SubmitUsersAsync(UsersDataModel userDto);
    Task<int> CreateUserAsync(UsersDataModel userDto);
    Task<ResponseDto> DeleteUsersAsync(int userId);
    Task<bool> DoesEmailExists(string email);
    Task<UsersDtoResponse?> GetUserByPhone(string phone);
    Task<UsersDataModel?> GetUserForLoginAsync(string email);
}