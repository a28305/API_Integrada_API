using Microsoft.AspNetCore.Mvc;
using MiApiIntegrada.Models;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    // Simulación de base de datos en memoria para el CRUD local
    private static List<User> _usuariosLocales = new List<User>();

    public UsersController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // --- MÉTODOS QUE CONSUMEN LA API EXTERNA ---

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
    }

    [HttpGet("externo/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
        var response = await client.GetAsync($"users/{id}");

        if (response.IsSuccessStatusCode)
        {
            var usuario = await response.Content.ReadFromJsonAsync<User>();
            return Ok(usuario);
        }
        return NotFound($"No se encontró el usuario con ID {id}");
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> GetUsersResumen()
    {
        var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
        var response = await client.GetAsync("users");

        if (response.IsSuccessStatusCode)
        {
            var usuarios = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();
            var resumen = usuarios.Select(u => new {
                u.Name,
                u.Email,
                Ciudad = u.Address?.City ?? "N/A"
            });
            return Ok(resumen);
        }
        return BadRequest("Error al procesar el resumen");
    }

    // --- MÉTODOS CRUD PARA DATOS LOCALES ---

    // 1. CREATE: Crear un usuario en nuestra lista local
    [HttpPost]
    public IActionResult CreateUser([FromBody] User nuevoUsuario)
    {
        nuevoUsuario.Id = _usuariosLocales.Count + 1; // Generamos un ID básico
        _usuariosLocales.Add(nuevoUsuario);
        
        // Retorna un 201 Created y la ubicación del nuevo recurso
        return CreatedAtAction(nameof(GetLocalUserById), new { id = nuevoUsuario.Id }, nuevoUsuario);
    }

    // 2. READ: Ver todos los usuarios locales
    [HttpGet("locales")]
    public IActionResult GetLocalUsers()
    {
        return Ok(_usuariosLocales);
    }

    // Auxiliar para el CreatedAtAction
    [HttpGet("locales/{id}")]
    public IActionResult GetLocalUserById(int id)
    {
        var usuario = _usuariosLocales.FirstOrDefault(u => u.Id == id);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    // 3. UPDATE: Actualizar un usuario local
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User usuarioActualizado)
    {
        var existente = _usuariosLocales.FirstOrDefault(u => u.Id == id);
        if (existente == null) return NotFound("Usuario local no encontrado");

        existente.Name = usuarioActualizado.Name;
        existente.Username = usuarioActualizado.Username;
        existente.Email = usuarioActualizado.Email;
        
        return NoContent(); // 204: Actualización exitosa, sin contenido que devolver
    }

    // 4. DELETE: Eliminar un usuario local
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var usuario = _usuariosLocales.FirstOrDefault(u => u.Id == id);
        if (usuario == null) return NotFound("Usuario no encontrado");

        _usuariosLocales.Remove(usuario);
        return Ok(new { message = $"Usuario con ID {id} eliminado correctamente" });
    }
}