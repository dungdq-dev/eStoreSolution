using ViewModels.Common;
using ViewModels.System.Languages;

namespace ApiIntegration
{
    public interface ILanguageApiClient
    {
        Task<ApiResponse<List<LanguageDto>>> GetAll();
    }
}