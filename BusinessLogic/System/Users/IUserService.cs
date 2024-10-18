using ViewModels.Common;
using ViewModels.System.Users;

namespace BusinessLogic.System.Users
{
    public interface IUserService
    {
        Task<ApiResponse<string>> Authenticate(LoginRequest request);

        Task<ApiResponse<bool>> Register(RegisterRequest request);

        Task<ApiResponse<bool>> Update(Guid id, UserUpdateRequest request);

        Task<PagedResponse<UserDto>> GetAllPaged(GetUserPagingRequest request);

        Task<ApiResponse<UserDto>> GetById(Guid id);

        Task<ApiResponse<UserDto>> GetByUsername(string username);

        Task<ApiResponse<bool>> Delete(Guid id);

        Task<ApiResponse<bool>> RoleAssign(Guid id, RoleAssignRequest request);
    }
}