using System.Net.Http.Json;
using System.Text.Json;
using NextWatchAI.Models;
using NextWatchAI.Services;

public class TmdbMovieService 
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public TmdbMovieService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = configuration["TMDB:ApiKey"] ?? throw new ArgumentNullException(nameof(_apiKey));
        _baseUrl = configuration["TMDB:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
    }

    public async Task<MovieListResult> GetPopularMoviesAsync(int page)
    {
        try
        {
            var uriBuilder = new UriBuilder($"{_baseUrl}movie/popular")
            {
                Query = $"api_key={_apiKey}&language=en-US&page={page}"
            };

            var response = await _httpClient.GetFromJsonAsync<TMDBMovieListResponse>(uriBuilder.Uri);

            if (response?.Results == null)
                return new MovieListResult();

            return new MovieListResult
            {
                Movies = response.Results.Select(m => new MovieVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    Overview = m.Overview,
                    PosterUrl = !string.IsNullOrEmpty(m.PosterPath)
                        ? $"https://image.tmdb.org/t/p/w200{m.PosterPath}"
                        : null
                }).ToList(),
                TotalPages = response.TotalPages
            };
        }
        catch
        {
            return new MovieListResult();
        }
    }

    public async Task<MovieListResult> SearchMoviesAsync(string query, int page)
    {
        try
        {
            var uriBuilder = new UriBuilder($"{_baseUrl}search/movie")
            {
                Query = $"api_key={_apiKey}&language=en-US&query={Uri.EscapeDataString(query)}&page={page}"
            };

            var response = await _httpClient.GetFromJsonAsync<TMDBMovieListResponse>(uriBuilder.Uri);

            if (response?.Results == null)
                return new MovieListResult();

            return new MovieListResult
            {
                Movies = response.Results.Select(m => new MovieVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    Overview = m.Overview,
                    PosterUrl = !string.IsNullOrEmpty(m.PosterPath)
                        ? $"https://image.tmdb.org/t/p/w780{m.PosterPath}"
                        : null
                }).ToList(),
                TotalPages = response.TotalPages
            };
        }
        catch
        {
            return new MovieListResult();
        }
    }

    public async Task<MovieVM?> GetMovieByIdAsync(int id)
    {
        var uriBuilder = new UriBuilder($"{_baseUrl}movie/{id}")
        {
            Query = $"api_key={_apiKey}&language=en-US"
        };

        var response = await _httpClient.GetAsync(uriBuilder.Uri);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var movieData = JsonSerializer.Deserialize<TMDBMovie>(json);

        if (movieData == null)
            return null;

        return new MovieVM
        {
            Id = movieData.Id,
            Title = movieData.Title,
            Overview = movieData.Overview,
            PosterUrl = string.IsNullOrEmpty(movieData.PosterPath)
                ? null
                : $"https://image.tmdb.org/t/p/w780{movieData.PosterPath}"
        };
    }
}
