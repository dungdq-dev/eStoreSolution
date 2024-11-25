using ViewModels.System.Roles;

namespace BusinessLogic.System.Roles
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetList();
    }
}