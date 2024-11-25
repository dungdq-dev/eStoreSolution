using AutoMapper;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ViewModels.Common;
using ViewModels.System.Languages;

namespace BusinessLogic.System.Languages
{
    public class LanguageService : ILanguageService
    {
        private readonly EStoreDbContext _context;
        private readonly IMapper _mapper;

        public LanguageService(EStoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<LanguageDto>>> GetList()
        {
            // execute stored procedure from sql server
            var query = await _context.Languages
                .FromSqlRaw("EXEC Sp_GetList_Languages")
                .IgnoreQueryFilters()
                .ToListAsync();

            // use automapper to map entity class with dto
            var languages = query
                .Select(_mapper.Map<Language, LanguageDto>).ToList();

            return new ApiSuccessResponse<List<LanguageDto>>(languages);
        }
    }
}