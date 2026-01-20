using Microsoft.AspNetCore.Mvc;
using MiApiIntegrada.Models;
using MiApiIntegrada.Models.Dto;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    // Base de datos en memoria
    private static List<User> _usuariosLocales = new List<User>();

    public UsersController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // --- MÉTODOS API EXTERNA CON DTO ---

    [HttpGet("externos")]
    public async Task<IActionResult> GetExternalUsers()
    {
        var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
        var response = await client.GetAsync("users");

        if (response.IsSuccessStatusCode)
        {
            var usuarios = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();
            
            // Mapeamos a UserDto para ocultar datos técnicos
            var resultado = usuarios.Select(u => new UserDto
            {
                Id = u.Id,
                NombreCompleto = u.Name,
                Email = u.Email
            });

            return Ok(resultado);
        }
        return BadRequest("Error al conectar con la API externa");
    }

    [HttpGet("externo/{id}")]
    public async Task<IActionResult> GetExternalUserById(int id)
    {
        var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
        var response = await client.GetAsync($"users/{id}");

        if (response.IsSuccessStatusCode)
        {
            var u = await response.Content.ReadFromJsonAsync<User>();
            var dto = new UserDto { Id = u.Id, NombreCompleto = u.Name, Email = u.Email };
            return Ok(dto);
        }
        return NotFound($"Usuario externo {id} no encontrado");
    }

    // --- MÉTODOS CRUD LOCAL CON DTO ---

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserCreateDto nuevoDto)
    {
        // Convertimos el DTO de entrada en nuestro modelo interno User
        var nuevoUsuario = new User
        {
            Id = _usuariosLocales.Count + 1,
            Name = nuevoDto.Name,
            Email = nuevoDto.Email,
            Username = nuevoDto.Username
        };

        _usuariosLocales.Add(nuevoUsuario);

        // Devolvemos un UserDto (salida limpia)
        var respuestaDto = new UserDto 
        { 
            Id = nuevoUsuario.Id, 
            NombreCompleto = nuevoUsuario.Name, 
            Email = nuevoUsuario.Email 
        };

        return CreatedAtAction(nameof(GetLocalUserById), new { id = respuestaDto.Id }, respuestaDto);
    }

    [HttpGet("locales")]
    public IActionResult GetLocalUsers()
    {
        var dtos = _usuariosLocales.Select(u => new UserDto
        {
            Id = u.Id,
            NombreCompleto = u.Name,
            Email = u.Email
        });
        return Ok(dtos);
    }

    [HttpGet("locales/{id}")]
    public IActionResult GetLocalUserById(int id)
    {
        var u = _usuariosLocales.FirstOrDefault(u => u.Id == id);
        if (u == null) return NotFound();

        var dto = new UserDto { Id = u.Id, NombreCompleto = u.Name, Email = u.Email };
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] UserCreateDto dtoActualizado)
    {
        var existente = _usuariosLocales.FirstOrDefault(u => u.Id == id);
        if (existente == null) return NotFound();

        existente.Name = dtoActualizado.Name;
        existente.Email = dtoActualizado.Email;
        existente.Username = dtoActualizado.Username;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var usuario = _usuariosLocales.FirstOrDefault(u => u.Id == id);
        if (usuario == null) return NotFound();

        _usuariosLocales.Remove(usuario);
        return Ok(new { message = $"Usuario {id} eliminado" });
    }
}