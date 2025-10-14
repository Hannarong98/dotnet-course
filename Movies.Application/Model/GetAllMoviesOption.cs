namespace Movies.Application.Model;

public class GetAllMoviesOption
{
    public required string? Title { get; set; }
 
    public required int? YearOfRelease { get; set; }

    public Guid? UserId { get; set; }
}