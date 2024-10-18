using ViewModels.Catalog.Categories;
using ViewModels.Common;

namespace BusinessLogic.Catalog.Categories
{
    public interface ICategoryService
    {
        Task<int> Create(CategoryCreateRequest request);

        Task<List<CategoryDto>> GetAll(string languageId);

        Task<PagedResponse<CategoryDto>> GetAllPaged(GetCategoryPagingRequest request);

        Task<ApiResponse<CategoryDto>> GetById(int categoryId, string languageId);

        Task<int> Update(CategoryUpdateRequest request);

        Task<int> Delete(int categoryId);
    }
}