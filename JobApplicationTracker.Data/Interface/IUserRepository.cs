using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.AuthDto;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IUserRepository
{
    Task<IEnumerable<UsersDtoResponse>> GetAllUsersAsync(int companyId);
    Task<UsersDtoResponse?> GetUserByEmail(string email);
    Task<UsersDtoResponse?> GetUsersByIdAsync(int userId);
    Task<ResponseDto> SubmitUsersAsync(UsersDataModel userDto);
    Task<ResponseDto> DeleteUsersAsync(int userId);
    Task<bool> DoesEmailExists(string email);
    Task<UsersDtoResponse?> GetUserByPhone(string phone);
    Task<UsersDataModel?> GetUserForLoginAsync(string email);
    Task<UsersProfileDto> GetUserProfileAsync(int userId);
    Task<ResponseDto> UploadUserProfilePictureAsync(int userId, string? imageUrl, string? bio);

    Task<UsersProfileDto?> GetUploadedProfileByIdAsync(int id);








}
