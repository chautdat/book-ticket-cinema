using System.Globalization;
using System.Text.Json;
using cinema.Models;

namespace cinema.Services
{
    public class TmdbMovieSyncService
    {
        private const string TmdbBaseUrl = "https://api.themoviedb.org/3";
        private const string TmdbImageBase = "https://image.tmdb.org/t/p/w500";

        private readonly HttpClient _httpClient;
        private readonly MyDbContext _db;
        private readonly IConfiguration _config;

        public TmdbMovieSyncService(HttpClient httpClient, MyDbContext db, IConfiguration config)
        {
            _httpClient = httpClient;
            _db = db;
            _config = config;
        }

        public async Task<SyncResult> SyncNowPlayingAsync(int maxMovies = 20, int pages = 2)
        {
            var apiKey = _config["TMDB_API_KEY"] ?? _config["Tmdb:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Thiếu TMDB_API_KEY hoặc Tmdb:ApiKey.");
            }

            var nowPlaying = await FetchNowPlayingAsync(apiKey, pages);
            var selected = nowPlaying
                .OrderByDescending(m => m.ReleaseDate ?? DateTime.MinValue)
                .Take(maxMovies)
                .ToList();

            var existingMovies = _db.Movies.ToList();
            var existingMap = existingMovies.ToDictionary(m => BuildKey(m.Title, m.ReleaseDate));
            var importedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var created = 0;
            var updated = 0;

            foreach (var item in selected)
            {
                JsonDocument? detail = null;
                try
                {
                    detail = await FetchMovieDetailsAsync(apiKey, item.Id);
                }
                catch
                {
                    continue;
                }

                using (detail)
                {
                    var movie = MapToMovie(item, detail);
                    var key = BuildKey(movie.Title, movie.ReleaseDate);

                    importedKeys.Add(key);

                    if (existingMap.TryGetValue(key, out var existing))
                    {
                        existing.Title = movie.Title;
                    existing.Description = movie.Description;
                    existing.Duration = movie.Duration;
                    existing.Genre = movie.Genre;
                    existing.ReleaseDate = movie.ReleaseDate;
                    existing.Age = movie.Age;
                    existing.Trailer = movie.Trailer;
                    existing.Director = movie.Director;
                    existing.Actor = movie.Actor;
                    existing.Publisher = movie.Publisher;
                    existing.Photo = movie.Photo;
                    existing.Status = true;
                    updated++;
                }
                else
                    {
                        _db.Movies.Add(movie);
                        created++;
                    }
                }
            }

            var disabled = 0;
            foreach (var movie in existingMovies)
            {
                var key = BuildKey(movie.Title, movie.ReleaseDate);
                if (!importedKeys.Contains(key) && movie.Status)
                {
                    movie.Status = false;
                    disabled++;
                }
            }

            await _db.SaveChangesAsync();

            return new SyncResult
            {
                Created = created,
                Updated = updated,
                Disabled = disabled,
                Total = selected.Count
            };
        }

        private async Task<List<TmdbMovieStub>> FetchNowPlayingAsync(string apiKey, int pages)
        {
            var movies = new List<TmdbMovieStub>();
            var pageCount = Math.Max(1, pages);

            for (var page = 1; page <= pageCount; page++)
            {
                var url = $"{TmdbBaseUrl}/movie/now_playing?api_key={apiKey}&language=vi-VN&region=VN&page={page}";
                var json = await _httpClient.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var item in results.EnumerateArray())
                {
                    var id = GetInt(item, "id");
                    if (id <= 0)
                    {
                        continue;
                    }

                    movies.Add(new TmdbMovieStub
                    {
                        Id = id,
                        Title = GetString(item, "title", "Đang cập nhật"),
                        Overview = GetString(item, "overview", "Đang cập nhật"),
                        PosterPath = GetString(item, "poster_path", string.Empty),
                        ReleaseDate = GetDate(item, "release_date"),
                        Adult = GetBool(item, "adult")
                    });
                }
            }

            return movies;
        }

        private async Task<JsonDocument> FetchMovieDetailsAsync(string apiKey, int movieId)
        {
            var url = $"{TmdbBaseUrl}/movie/{movieId}?api_key={apiKey}&language=vi-VN&append_to_response=credits,videos";
            var json = await _httpClient.GetStringAsync(url);
            return JsonDocument.Parse(json);
        }

        private Movie MapToMovie(TmdbMovieStub stub, JsonDocument detailDoc)
        {
            var root = detailDoc.RootElement;

            var title = GetString(root, "title", stub.Title);
            var description = GetString(root, "overview", stub.Overview);
            var posterPath = GetString(root, "poster_path", stub.PosterPath);
            var photo = string.IsNullOrWhiteSpace(posterPath) ? string.Empty : $"{TmdbImageBase}{posterPath}";

            var releaseDate = GetDate(root, "release_date") ?? stub.ReleaseDate ?? DateTime.Today;
            var runtime = GetInt(root, "runtime");
            var duration = runtime > 0 ? runtime.ToString(CultureInfo.InvariantCulture) : "120";

            var genres = GetGenres(root);
            var director = GetDirector(root);
            var actors = GetActors(root);
            var trailer = GetTrailer(root);

            return new Movie
            {
                Title = title,
                Description = string.IsNullOrWhiteSpace(description) ? "Đang cập nhật" : description,
                Duration = duration,
                Genre = string.IsNullOrWhiteSpace(genres) ? "Phim" : genres,
                ReleaseDate = releaseDate,
                Age = (stub.Adult || GetBool(root, "adult")) ? 18 : 13,
                Trailer = trailer,
                Director = string.IsNullOrWhiteSpace(director) ? "Đang cập nhật" : director,
                Actor = string.IsNullOrWhiteSpace(actors) ? "Đang cập nhật" : actors,
                Publisher = "TMDB",
                Status = true,
                Photo = photo
            };
        }

        private static string GetGenres(JsonElement root)
        {
            if (!root.TryGetProperty("genres", out var genresEl) || genresEl.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var names = new List<string>();
            foreach (var g in genresEl.EnumerateArray())
            {
                var name = GetString(g, "name", string.Empty);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }
            }

            return string.Join(", ", names.Take(3));
        }

        private static string GetDirector(JsonElement root)
        {
            if (!root.TryGetProperty("credits", out var credits) || credits.ValueKind != JsonValueKind.Object)
            {
                return string.Empty;
            }

            if (!credits.TryGetProperty("crew", out var crewEl) || crewEl.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            foreach (var crew in crewEl.EnumerateArray())
            {
                var job = GetString(crew, "job", string.Empty);
                if (string.Equals(job, "Director", StringComparison.OrdinalIgnoreCase))
                {
                    return GetString(crew, "name", string.Empty);
                }
            }

            return string.Empty;
        }

        private static string GetActors(JsonElement root)
        {
            if (!root.TryGetProperty("credits", out var credits) || credits.ValueKind != JsonValueKind.Object)
            {
                return string.Empty;
            }

            if (!credits.TryGetProperty("cast", out var castEl) || castEl.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var names = new List<string>();
            foreach (var cast in castEl.EnumerateArray())
            {
                var name = GetString(cast, "name", string.Empty);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }

                if (names.Count >= 5)
                {
                    break;
                }
            }

            return string.Join(", ", names);
        }

        private static string GetTrailer(JsonElement root)
        {
            if (!root.TryGetProperty("videos", out var videos) || videos.ValueKind != JsonValueKind.Object)
            {
                return string.Empty;
            }

            if (!videos.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            foreach (var video in results.EnumerateArray())
            {
                var type = GetString(video, "type", string.Empty);
                var site = GetString(video, "site", string.Empty);
                if (!string.Equals(type, "Trailer", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(site, "YouTube", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var key = GetString(video, "key", string.Empty);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    return $"https://www.youtube.com/watch?v={key}";
                }
            }

            return string.Empty;
        }

        private static string BuildKey(string title, DateTime releaseDate)
        {
            return $"{title.Trim().ToLowerInvariant()}|{releaseDate:yyyy-MM-dd}";
        }

        private static string GetString(JsonElement element, string property, string fallback)
        {
            if (element.TryGetProperty(property, out var prop) && prop.ValueKind == JsonValueKind.String)
            {
                return prop.GetString() ?? fallback;
            }

            return fallback;
        }

        private static int GetInt(JsonElement element, string property)
        {
            if (element.TryGetProperty(property, out var prop) && prop.ValueKind == JsonValueKind.Number)
            {
                if (prop.TryGetInt32(out var value))
                {
                    return value;
                }
            }

            return 0;
        }

        private static bool GetBool(JsonElement element, string property)
        {
            if (element.TryGetProperty(property, out var prop) && prop.ValueKind == JsonValueKind.True)
            {
                return true;
            }

            if (element.TryGetProperty(property, out prop) && prop.ValueKind == JsonValueKind.False)
            {
                return false;
            }

            return false;
        }

        private static DateTime? GetDate(JsonElement element, string property)
        {
            if (element.TryGetProperty(property, out var prop) && prop.ValueKind == JsonValueKind.String)
            {
                var raw = prop.GetString();
                if (!string.IsNullOrWhiteSpace(raw) &&
                    DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date.Date;
                }
            }

            return null;
        }

        private sealed class TmdbMovieStub
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Overview { get; set; } = string.Empty;
            public string PosterPath { get; set; } = string.Empty;
            public DateTime? ReleaseDate { get; set; }
            public bool Adult { get; set; }
        }

        public sealed class SyncResult
        {
            public int Created { get; set; }
            public int Updated { get; set; }
            public int Disabled { get; set; }
            public int Total { get; set; }
        }
    }
}
