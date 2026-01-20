using Microsoft.AspNetCore.Mvc;
using MiApiIntegrada.Models;
using MiApiIntegrada.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    // Simulación de base de datos local para Posts
    private static List<Post> _postsLocales = new List<Post>();

    public PostsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // --- MÉTODOS EXTERNOS (API) ---

    [HttpGet("usuario-externo/{userId}")]
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
        return BadRequest("No se pudieron obtener los posts externos.");
    }

    // --- MÉTODOS CRUD LOCALES ---

    // 1. CREATE: Crear un post local
    [HttpPost]
    public IActionResult CreateLocalPost([FromBody] Post nuevoPost)
    {
        nuevoPost.Id = _postsLocales.Count + 1;
        _postsLocales.Add(nuevoPost);
        
        var dto = new PostDto { 
            Id = nuevoPost.Id, 
            Titulo = nuevoPost.Title, 
            Contenido = nuevoPost.Body 
        };

        return CreatedAtAction(nameof(GetLocalPostById), new { id = dto.Id }, dto);
    }

    // 2. READ: Obtener todos los posts locales
    [HttpGet("locales")]
    public IActionResult GetLocalPosts()
    {
        var dtos = _postsLocales.Select(p => new PostDto {
            Id = p.Id,
            Titulo = p.Title,
            Contenido = p.Body
        });
        return Ok(dtos);
    }

    // Obtener un post local por ID (Auxiliar)
    [HttpGet("locales/{id}")]
    public IActionResult GetLocalPostById(int id)
    {
        var p = _postsLocales.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound("Post local no encontrado.");

        var dto = new PostDto { Id = p.Id, Titulo = p.Title, Contenido = p.Body };
        return Ok(dto);
    }

    // 3. UPDATE: Actualizar post local
    [HttpPut("{id}")]
    public IActionResult UpdateLocalPost(int id, [FromBody] Post postActualizado)
    {
        var existente = _postsLocales.FirstOrDefault(x => x.Id == id);
        if (existente == null) return NotFound();

        existente.Title = postActualizado.Title;
        existente.Body = postActualizado.Body;

        return NoContent();
    }

    // 4. DELETE: Borrar post local
    [HttpDelete("{id}")]
    public IActionResult DeleteLocalPost(int id)
    {
        var post = _postsLocales.FirstOrDefault(x => x.Id == id);
        if (post == null) return NotFound();

        _postsLocales.Remove(post);
        return Ok(new { message = $"Post local {id} eliminado correctamente." });
    }
}