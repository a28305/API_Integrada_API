using Microsoft.AspNetCore.Mvc;
using MiApiIntegrada.Models;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UsersController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // GET: api/users/externos (Trae los 10 usuarios de JSONPlaceholder)
    [HttpGet("externos")]
    public async Task<IActionResult> GetExternalUsers()
    {
        var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
        var response = await client.GetAsync("users");

        if (response.IsSuccessStatusCode)
        {
            var usuarios = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();
            return Ok(usuarios);
        }
        return BadRequest("Error al conectar con la API de usuarios");
    }// GET: api/users/externo/5
[HttpGet("externo/{id}")]
public async Task<IActionResult> GetUserById(int id)
{
    var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
    
    // Concatenamos el ID a la URL: users/5
    var response = await client.GetAsync($"users/{id}");

    if (response.IsSuccessStatusCode)
    {
        var usuario = await response.Content.ReadFromJsonAsync<User>();
        return Ok(usuario);
    }
    
    return NotFound($"No se encontró el usuario con ID {id}");
}

    // GET: api/users/resumen (Un endpoint propio que solo devuelve nombres y correos)
    [HttpGet("resumen")]
    public async Task<IActionResult> GetUsersResumen()
    {
        var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
        var response = await client.GetAsync("users");

        if (response.IsSuccessStatusCode)
        {
            var usuarios = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();
            
            // Transformamos los datos (LINQ) para devolver solo lo que queremos
            var resumen = usuarios.Select(u => new {
                u.Name,
                u.Email,
                Ciudad = u.Address.City
            });

            return Ok(resumen);
        }
        return BadRequest("Error al procesar el resumen");
    }
}