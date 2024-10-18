using ViewModels.Common;
using ViewModels.System.Roles;

namespace ApiIntegration
{
    public interface IRoleApiClient
    {
        Task<ApiResponse<List<RoleDto>>> GetAll();
    }
}