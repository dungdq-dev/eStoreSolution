using ViewModels.Common;
using ViewModels.System.Languages;

namespace BusinessLogic.System.Languages
{
    public interface ILanguageService
    {
        Task<ApiResponse<List<LanguageDto>>> GetList();
    }
}