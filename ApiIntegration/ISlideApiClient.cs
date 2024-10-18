using ViewModels.Utilities.Slides;

namespace ApiIntegration
{
    public interface ISlideApiClient
    {
        Task<List<SlideDto>> GetAll();
    }
}