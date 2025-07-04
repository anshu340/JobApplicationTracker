
using JobApplicationTracke.Data.Dto;
namespace JobApplicationTracke.Data.Interface;

public interface IUserRepository
{
    Task<IEnumerable<UsersDto>> GetAllUsersAsync();
    Task<UsersDto?> GetUserByEmail(string email);
    Task<UsersDto> GetUsersByIdAsync(int userId);
    Task<ResponseDto> SubmitUsersAsync(UsersDto userDto);
    Task<ResponseDto> DeleteUsersAsync(int userId);
    Task<ResponseDto> CreateUserAsync(SignupDto credentials);
    Task<bool> DoesEmailExists(string email);
}