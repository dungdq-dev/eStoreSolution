using ViewModels.Common;
using ViewModels.System.Users;

namespace ApiIntegration
{
    public interface IUserApiClient
    {
        Task<ApiResponse<string>> Authenticate(LoginRequest request);

        Task<PagedResponse<UserDto>> GetAll(GetUserPagingRequest request);

        Task<ApiResponse<bool>> Register(RegisterRequest registerRequest);

        Task<ApiResponse<UserDto>> GetById(Guid id);

        Task<ApiResponse<UserDto>> GetByName(string username);

        Task<ApiResponse<bool>> Update(Guid id, UserUpdateRequest request);

        Task<ApiResponse<bool>> RoleAssign(Guid id, RoleAssignRequest request);

        Task<ApiResponse<bool>> Delete(Guid id);
    }
}