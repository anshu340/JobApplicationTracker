
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.AuthDto;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IUserRepository
{
    Task<IEnumerable<UsersDataModel>> GetAllUsersAsync();
    Task<UsersDataModel?> GetUserByEmail(string email);
    Task<UsersDataModel> GetUsersByIdAsync(int userId);
    Task<ResponseDto> SubmitUsersAsync(UsersDataModel userDto);
    Task<ResponseDto> DeleteUsersAsync(int userId);
    Task<ResponseDto> CreateUserAsync(SignUpDto credentials);
    Task<bool> DoesEmailExists(string email);
}