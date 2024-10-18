using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using ViewModels.Utilities.Slides;

namespace BusinessLogic.Utilities.Slides
{
    public class SlideService : ISlideService
    {
        private readonly EStoreDbContext _context;

        public SlideService(EStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<SlideDto>> GetAll()
        {
            var slides = await _context.Slides.OrderBy(x => x.SortOrder)
                .Select(x => new SlideDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Url = x.Url,
                    Image = x.Image
                }).ToListAsync();

            return slides;
        }
    }
}