using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using ViewModels.Common;
using ViewModels.System.Languages;

namespace BusinessLogic.System.Languages
{
    public class LanguageService : ILanguageService
    {
        private readonly EStoreDbContext _context;

        public LanguageService(EStoreDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<LanguageDto>>> GetAll()
        {
            var languages = await _context.Languages.Select(x => new LanguageDto()
            {
                Id = x.Id,
                Name = x.Name,
                IsDefault = x.IsDefault
            }).ToListAsync();

            return new ApiSuccessResponse<List<LanguageDto>>(languages);
        }
    }
}