using ViewModels.Utilities.Slides;

namespace BusinessLogic.Utilities.Slides
{
    public interface ISlideService
    {
        Task<List<SlideDto>> GetAll();
    }
}