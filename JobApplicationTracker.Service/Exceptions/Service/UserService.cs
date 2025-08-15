using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.Services.Interfaces;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResponseDto> SubmitUserAsync(UsersDataModel userDto)
    {
        // If password is set (changed), hash it before passing to repo
        if (!string.IsNullOrEmpty(userDto.PasswordHash))
        {
            userDto.PasswordHash = _passwordHasher.HashPassword(userDto.PasswordHash);
        }

        return await _userRepository.SubmitUsersAsync(userDto);
    }
}
