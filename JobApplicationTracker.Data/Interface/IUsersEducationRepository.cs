using JobApplicationTracker.Dto;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface
{
    public interface IUsersEducationRepository
    {
        Task<IEnumerable<UsersEducationDto>> GetAllUsersEducationAsync();

        Task<UsersEducationDto> GetUsersEducationByIdAsync(int usersEducationId);

        Task<ResponseDto> SubmitUsersEducationAsync(UsersEducationDto usersEducationDto);

        Task<ResponseDto> DeleteUsersEducationAsync(int usersEducationId);
    }
}
