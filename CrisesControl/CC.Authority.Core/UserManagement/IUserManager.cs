using CC.Authority.Core.UserManagement.Models;

namespace CC.Authority.Core.UserManagement
{
    public interface IUserManager
    {
        Task<UserResponse> AddUser(UserInput userInput);
        Task<(bool, UserResponse?)> UserExists(string email, string externalScimId);
        Task<UserResponse> GetUser(string id);
        Task<UserResponse> UpdateUser(UserInput userInput);
    }
}