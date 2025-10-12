using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [Authorize(Policy = Roles.Write)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<MovieResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie();
        await movieService.CreateAsync(movie, token);

        var response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id },response);
    }

    [HttpGet(ApiEndpoints.Movies.GetByIdOrSlug)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var user = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id, user, token)
            : await movieService.GetBySlugAsync(idOrSlug, user, token);
                

        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        
        return Ok(response);
    }
    
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        
        var user = HttpContext.GetUserId();
        
        var movies = await movieService.GetAllAsync(user, token);

        var response = movies.MapToResponse();
        
        return Ok(response);
    }

    [Authorize(Policy = Roles.Write)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        
        var user = HttpContext.GetUserId();
        
        var movie = request.MapToMovie(id);
        var updatedMovie = await movieService.UpdateAsync(movie, user, token);

        if (updatedMovie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();

        return Ok(response);
    }

    [Authorize(Policy = Roles.Write)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await movieService.DeleteByIdAsync(id, token);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}