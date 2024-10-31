using AutoMapper;
using Common.Constants;
using Common.Exceptions;
using Common.Helpers;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using ViewModels.Catalog.Categories;
using ViewModels.Common;

namespace BusinessLogic.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly EStoreDbContext _context;

        public CategoryService(EStoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Create(CategoryCreateRequest request)
        {
            var languages = await _context.Languages.ToListAsync();
            var translations = new List<CategoryTranslation>();
            foreach (var language in languages)
            {
                if (language.Id == request.LanguageId)
                {
                    translations.Add(new CategoryTranslation()
                    {
                        Name = request.Name,
                        SeoAlias = request.Name.GenerateSlug(),
                        SeoDescription = request.SeoDescription,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    });
                }
                else
                {
                    translations.Add(new CategoryTranslation()
                    {
                        Name = SystemConstants.CatalogConstants.NA,
                        SeoAlias = SystemConstants.CatalogConstants.NA,
                        SeoDescription = SystemConstants.CatalogConstants.NA,
                        SeoTitle = SystemConstants.CatalogConstants.NA,
                        LanguageId = language.Id
                    });
                }
            }

            var category = new Category()
            {
                SortOrder = request.SortOrder,
                IsShowOnHome = request.IsShowOnHome,
                CategoryTranslations = translations
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<int> Delete(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category", categoryId);

            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns>CategoryDto</returns>
        public async Task<List<CategoryDto>> GetAll(string languageId)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId
                        select new { c, ct };

            return await query.Select(x => new CategoryDto()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                SeoAlias = x.ct.SeoAlias,
                SeoTitle = x.ct.SeoTitle,
                SeoDescription = x.ct.SeoDescription,
                LanguageId = x.ct.LanguageId,
                IsShowOnHome = x.c.IsShowOnHome,
                SortOrder = x.c.SortOrder,
                Status = x.c.Status,
                ParentId = x.c.ParentId
            }).ToListAsync();
        }

        public async Task<PagedResponse<CategoryDto>> GetAllPaged(GetCategoryPagingRequest request)
        {
            //1. Select join
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where ct.LanguageId == request.LanguageId
                        select new { c, ct };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.ct.Name.Contains(request.Keyword));

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CategoryDto()
                {
                    Id = x.c.Id,
                    Name = x.ct.Name,
                    SeoAlias = x.ct.SeoAlias,
                    SeoDescription = x.ct.SeoDescription,
                    SeoTitle = x.ct.SeoTitle,
                    SortOrder = x.c.SortOrder,
                    IsShowOnHome = x.c.IsShowOnHome,
                    ParentId = x.c.ParentId,
                    Status = x.c.Status
                }).ToListAsync();

            //4. Select and projection
            var result = new PagedResponse<CategoryDto>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = data
            };
            return result;
        }

        public async Task<ApiResponse<CategoryDto>> GetById(int categoryId, string languageId)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId && c.Id == categoryId
                        select new { c, ct };

            var result = await query.Select(x => new CategoryDto()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                SeoDescription = x.ct.SeoDescription,
                SeoTitle = x.ct.SeoTitle,
                SeoAlias = x.ct.SeoAlias,
                SortOrder = x.c.SortOrder,
                IsShowOnHome = x.c.IsShowOnHome,
                Status = x.c.Status,
                ParentId = x.c.ParentId
            }).FirstOrDefaultAsync();

            if (result == null)
            {
                return new ApiErrorResponse<CategoryDto>("Category not found");
            }
            return new ApiSuccessResponse<CategoryDto>(result);
        }

        public async Task<int> Update(CategoryUpdateRequest request)
        {
            var category = await _context.Categories.FindAsync(request.Id);
            var categoryTranslations = await _context.CategoryTranslations
                .FirstOrDefaultAsync(x => x.CategoryId == request.Id && x.LanguageId == request.LanguageId);

            if (category == null || categoryTranslations == null)
                throw new NotFoundException("Category", request.Id);

            categoryTranslations.Name = request.Name;
            categoryTranslations.SeoAlias = request.Name.GenerateSlug();
            categoryTranslations.SeoDescription = request.SeoDescription;
            categoryTranslations.SeoTitle = request.SeoTitle;
            category.SortOrder = request.SortOrder;
            category.IsShowOnHome = request.IsShowOnHome;
            category.Status = request.Status;
            category.ParentId = request.ParentId;

            return await _context.SaveChangesAsync();
        }
    }
}