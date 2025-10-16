using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers.V1;

[ApiController]
public class RatingsController(IRatingService ratingService) : ControllerBase
{
    [Authorize]
    [HttpPut(ApiEndpoints.V1.Movies.Rate)]
    [EndpointDescription("Rate a movie")]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var result = await ratingService.RateMovieAsync(id, userId!.Value, request.Rating, token);

        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpDelete(ApiEndpoints.V1.Movies.DeleteRating)]
    [EndpointDescription("Delete user rating by user id")]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var result = await ratingService.DeleteRatingAsync(id, userId!.Value, token);

        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.V1.Ratings.GetUserRatings)]
    [EndpointDescription("Get all rating from authenticated user")]
    public async Task<IActionResult> GetUserRatings(CancellationToken token = default)
    {
        var user = HttpContext.GetUserId();

        var ratings = await ratingService.GetRatingsForUserAsync(user!.Value, token);
        
        return Ok(ratings.MapToResponse());
    }
}