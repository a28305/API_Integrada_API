using Microsoft.AspNetCore.Mvc;
using MiApiIntegrada.Models;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PostsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("usuario/{userId}")]
    public async Task<IActionResult> GetPostsByUser(int userId)
    {
        var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
        var response = await client.GetAsync($"posts?userId={userId}");

        if (response.IsSuccessStatusCode)
        {
            var posts = await response.Content.ReadFromJsonAsync<IEnumerable<Post>>();
            var dtos = posts.Select(p => new PostDto {
                Id = p.Id,
                Titulo = p.Title,
                Contenido = p.Body
            });
            return Ok(dtos);
        }
        return BadRequest("No se pudieron obtener los posts.");
    }
}