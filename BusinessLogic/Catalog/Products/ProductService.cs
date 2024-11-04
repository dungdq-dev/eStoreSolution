using BusinessLogic.Common.FileSystem;
using Common.Constants;
using Common.Exceptions;
using Common.Helpers;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using ViewModels.Catalog.ProductImages;
using ViewModels.Catalog.Products;
using ViewModels.Common;

namespace BusinessLogic.Catalog.Products
{
    public class ProductService : IProductService
    {
        private readonly EStoreDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductService(EStoreDbContext context, IStorageService storageService, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _storageService = storageService;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// thêm ảnh sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = productId,
                SortOrder = request.SortOrder
            };

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();
            return productImage.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task AddViewcount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
                throw new NotFoundException("Product", productId);

            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// thêm mới sản phẩm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<int> Create(ProductCreateRequest request)
        {
            var languages = _context.Languages;
            var translations = new List<ProductTranslation>();
            foreach (var language in languages)
            {
                if (language.Id == request.LanguageId)
                {
                    translations.Add(new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoDescription = request.SeoDescription,
                        SeoTitle = request.SeoTitle,
                        SeoAlias = request.Name.GenerateSlug(),
                        LanguageId = request.LanguageId
                    });
                }
                else
                {
                    translations.Add(new ProductTranslation()
                    {
                        Name = SystemConstants.CatalogConstants.NA,
                        Description = SystemConstants.CatalogConstants.NA,
                        Details = SystemConstants.CatalogConstants.NA,
                        SeoDescription = SystemConstants.CatalogConstants.NA,
                        SeoTitle = SystemConstants.CatalogConstants.NA,
                        SeoAlias = SystemConstants.CatalogConstants.NA,
                        LanguageId = language.Id
                    });
                }
            }
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = translations
            };

            // Save image
            string webRootPath = _hostingEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            string fullPath = Path.Combine(webRootPath, SystemConstants.UserContentFolderName);
            // Create user-content folder if it doesn't exists
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var images = _context.ProductImages.Where(i => i.ProductId == productId);
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedResponse<ProductDto>> GetAll(GetProductPagingRequest request)
        {
            //1. Select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pc in _context.ProductCategories on p.Id equals pc.ProductId into pc1
                        from pc in pc1.DefaultIfEmpty()
                        join c in _context.Categories on pc.CategoryId equals c.Id into pc2
                        from c in pc2.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into pi1
                        from pi in pi1.DefaultIfEmpty()
                        where pt.LanguageId == request.LanguageId && pi.IsDefault
                        select new { p, pt, pc, pi };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));

            if (request.CategoryId != null && request.CategoryId != 0)
            {
                query = query.Where(p => p.pc.CategoryId == request.CategoryId);
            }

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductDto()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).Distinct().ToListAsync();

            //4. Select and projection
            var pagedResult = new PagedResponse<ProductDto>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = data
            };
            return pagedResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public async Task<ApiResponse<ProductDto>> GetById(int productId, string languageId)
        {
            var product = await _context.Products.FindAsync(productId);

            var productTranslation = await _context.ProductTranslations
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.LanguageId == languageId);

            var categories = await (from c in _context.Categories
                                    join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                                    join pc in _context.ProductCategories on c.Id equals pc.CategoryId
                                    where pc.ProductId == productId && ct.LanguageId == languageId
                                    select ct.Name).ToListAsync();

            var image = await _context.ProductImages.Where(x => x.ProductId == productId && x.IsDefault).FirstOrDefaultAsync();

            if (product == null)
            {
                return new ApiErrorResponse<ProductDto>($"Product with id ({productId}) not found");
            }

            var productViewModel = new ProductDto()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = productTranslation != null ? productTranslation.Description : "",
                LanguageId = productTranslation.LanguageId,
                Details = productTranslation.Details,
                Name = productTranslation.Name,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                SeoAlias = productTranslation.SeoAlias,
                SeoDescription = productTranslation.SeoDescription,
                SeoTitle = productTranslation.SeoTitle,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                Categories = categories,
                ThumbnailImage = image != null ? image.ImagePath : "no-image.jpg"
            };

            return new ApiSuccessResponse<ProductDto>(productViewModel);
        }

        /// <summary>
        /// lấy 1 ảnh của sản phẩm bằng id
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ProductImageDto> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                throw new NotFoundException("Image", imageId);

            var model = new ProductImageDto()
            {
                Id = image.Id,
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };

            return model;
        }

        /// <summary>
        /// lấy danh sách ảnh sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<List<ProductImageDto>> GetListImages(int productId)
        {
            return await _context.ProductImages.Where(x => x.ProductId == productId)
                .Select(i => new ProductImageDto()
                {
                    Id = i.Id,
                    Caption = i.Caption,
                    DateCreated = i.DateCreated,
                    FileSize = i.FileSize,
                    ImagePath = i.ImagePath,
                    IsDefault = i.IsDefault,
                    ProductId = i.ProductId,
                    SortOrder = i.SortOrder
                }).ToListAsync();
        }

        /// <summary>
        /// xóa ảnh sản phẩm
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<int> RemoveImage(int imageId)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new NotFoundException("Image", imageId);
            _context.ProductImages.Remove(productImage);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// cập nhật thông tin sản phẩm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslations =
                await _context.ProductTranslations
                .FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId);

            if (product == null || productTranslations == null)
                throw new NotFoundException("Product", request.Id);

            productTranslations.Name = request.Name;
            productTranslations.Description = request.Description;
            productTranslations.Details = request.Details;
            productTranslations.SeoDescription = request.SeoDescription;
            productTranslations.SeoTitle = request.SeoTitle;
            productTranslations.SeoAlias = request.Name.GenerateSlug();
            productTranslations.LanguageId = request.LanguageId;
            product.Price = request.Price;
            product.IsFeatured = request.IsFeatured;

            //Save image
            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault && i.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await SaveFile(request.ThumbnailImage);
                    _context.ProductImages.Update(thumbnailImage);
                }
            }

            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new NotFoundException("Image", imageId);

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Update(productImage);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// cập nhật số lượng hàng tồn kho
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<bool> UpdateStock(int productId, int amount)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new NotFoundException("Product", productId);
            product.Stock += amount;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/" + SystemConstants.UserContentFolderName + "/" + fileName;
        }

        /// <summary>
        /// lấy tất cả sản phẩm thuộc danh mục được chọn
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedResponse<ProductDto>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request)
        {
            //1. Select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pc in _context.ProductCategories on p.Id equals pc.ProductId
                        join c in _context.Categories on pc.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pc };
            //2. filter
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(p => p.pc.CategoryId == request.CategoryId);
            }
            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductDto()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount
                }).ToListAsync();

            //4. Select and projection
            var pagedResult = new PagedResponse<ProductDto>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = data
            };
            return pagedResult;
        }

        /// <summary>
        /// gán danh mục cho sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ApiResponse<bool>> CategoryAssign(int productId, CategoryAssignRequest request)
        {
            var user = await _context.Products.FindAsync(productId);
            if (user == null)
            {
                return new ApiErrorResponse<bool>($"Sản phẩm với id {productId} không tồn tại");
            }
            foreach (var category in request.Categories)
            {
                var productInCategory = await _context.ProductCategories
                    .FirstOrDefaultAsync(x => x.CategoryId == int.Parse(category.Id)
                    && x.ProductId == productId);
                if (productInCategory != null && !category.Selected)
                {
                    _context.ProductCategories.Remove(productInCategory);
                }
                else if (productInCategory == null && category.Selected)
                {
                    await _context.ProductCategories.AddAsync(new ProductCategory()
                    {
                        CategoryId = int.Parse(category.Id),
                        ProductId = productId
                    });
                }
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResponse<bool>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<List<ProductDto>> GetFeaturedProducts(string languageId, int take)
        {
            //1. Select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && (pi == null || pi.IsDefault)
                        && p.IsFeatured == true
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductDto()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).Distinct().OrderBy(x => x.DateCreated).ToListAsync();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<List<ProductDto>> GetLatestProducts(string languageId, int take)
        {
            //1. Select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pc in _context.ProductCategories on p.Id equals pc.ProductId into ppic
                        from pc in ppic.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        join c in _context.Categories on pc.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && (pi == null || pi.IsDefault)
                        select new { p, pt, pc, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductDto()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).Distinct().OrderBy(x => x.DateCreated).ToListAsync();

            return data;
        }
    }
}