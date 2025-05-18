using NextWatchAI.Models;

namespace NextWatchAI.Services
{
    public interface IAiRecommendationService
    {
        Task<AiRecommendationResult> RecommendMovieAsync(string mood);
    }
}
