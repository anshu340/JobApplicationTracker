using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using System.Threading.Tasks;

namespace JobApplicationTracker.Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDto> SubmitUserAsync(UsersDataModel userDto);
    }

}
