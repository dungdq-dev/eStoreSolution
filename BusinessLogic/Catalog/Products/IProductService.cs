using ViewModels.Catalog.ProductImages;
using ViewModels.Catalog.Products;
using ViewModels.Common;

namespace BusinessLogic.Catalog.Products
{
    public interface IProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<PagedResponse<ProductDto>> GetListPaged(GetProductPagingRequest request);

        Task<ApiResponse<ProductDto>> GetById(int productId, string languageId);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> Delete(int productId);

        Task<bool> UpdateStock(int productId, int amount);

        Task AddViewcount(int productId);

        Task<PagedResponse<ProductDto>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request);

        Task<ApiResponse<bool>> CategoryAssign(int productId, CategoryAssignRequest request);

        Task<List<ProductDto>> GetFeaturedProducts(string languageId, int take);

        Task<List<ProductDto>> GetLatestProducts(string languageId, int take);

        Task<int> AddImage(int productId, ProductImageCreateRequest request);

        Task<List<ProductImageDto>> GetListImages(int productId);

        Task<ProductImageDto> GetImageById(int imageId);

        Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request);

        Task<int> RemoveImage(int imageId);
    }
}