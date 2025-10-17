namespace Movies.Api;

public abstract class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class V1
    {
        private const string VersionBase = $"{ApiBase}/v1";

        public static class Movies
        {
            private const string MoviesBase = $"{VersionBase}/movies";


            public const string GetByIdOrSlug = $"{MoviesBase}/{{idOrSlug}}";
            public const string GetAll = MoviesBase;
            public const string Create = MoviesBase;
            public const string Update = $"{MoviesBase}/{{id:guid}}";
            public const string Delete = $"{MoviesBase}/{{id:guid}}";


            public const string Rate = $"{MoviesBase}/{{id:guid}}/ratings";
            public const string DeleteRating = $"{MoviesBase}/{{id:guid}}/ratings";
        }

        public static class Ratings
        {
            private const string RatingsBase = $"{VersionBase}/ratings";

            public const string GetUserRatings = $"{RatingsBase}/me";
        }
    }
}