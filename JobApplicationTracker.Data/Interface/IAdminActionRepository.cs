
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface IAdminActionRepository
{
    Task<IEnumerable<AdminActionsDto>> GetAllAdminActionAsync();
    Task<AdminActionsDto> GetAdminActionByIdAsync(int adminActionId);
    Task<ResponseDto> SubmitAdminActionAsync(AdminActionsDto adminActionDto);
    Task<ResponseDto> DeleteAdminActionAsync(int actionId);
}


