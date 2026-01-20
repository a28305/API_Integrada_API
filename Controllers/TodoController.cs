using Microsoft.AspNetCore.Mvc;
using MiApiIntegrada.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TodosController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("usuario/{userId}")]
    public async Task<IActionResult> GetUserTodos(int userId)
    {
        var client = _httpClientFactory.CreateClient("JsonPlaceholderApi");
        var response = await client.GetAsync($"users/{userId}/todos");

        if (response.IsSuccessStatusCode)
        {
            var todos = await response.Content.ReadFromJsonAsync<IEnumerable<Todo>>();
            var dtos = todos.Select(t => new TodoDto {
                Tarea = t.Title,
                Estado = t.Completed ? "Completada ✅" : "Pendiente ⏳"
            });
            return Ok(dtos);
        }
        return BadRequest("No se pudieron obtener las tareas.");
    }
}