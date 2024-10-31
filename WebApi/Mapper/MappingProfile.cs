using AutoMapper;
using DataAccess.Entities;
using ViewModels.Catalog.Categories;
using ViewModels.System.Roles;
using ViewModels.System.Users;

namespace WebApi.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, AppUser>();
            CreateMap<AppUser, UserDto>();

            CreateMap<RoleDto, AppRole>();
            CreateMap<AppRole, RoleDto>();

            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();
        }
    }
}
