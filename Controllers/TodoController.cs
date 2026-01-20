using Microsoft.AspNetCore.Mvc;
using MiApiIntegrada.Models;
using MiApiIntegrada.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    // Simulación de base de datos local para Tareas (Todos)
    private static List<Todo> _todosLocales = new List<Todo>();

    public TodosController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // --- MÉTODOS EXTERNOS (API) ---

    [HttpGet("usuario-externo/{userId}")]
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
        return BadRequest("No se pudieron obtener las tareas externas.");
    }

    // --- MÉTODOS CRUD LOCALES ---

    // 1. CREATE: Crear una tarea local
    [HttpPost]
    public IActionResult CreateLocalTodo([FromBody] Todo nuevoTodo)
    {
        nuevoTodo.Id = _todosLocales.Count + 1;
        _todosLocales.Add(nuevoTodo);
        
        var dto = new TodoDto { 
            Tarea = nuevoTodo.Title, 
            Estado = nuevoTodo.Completed ? "Completada ✅" : "Pendiente ⏳"
        };

        return CreatedAtAction(nameof(GetLocalTodoById), new { id = nuevoTodo.Id }, dto);
    }

    // 2. READ: Ver todas las tareas locales
    [HttpGet("locales")]
    public IActionResult GetLocalTodos()
    {
        var dtos = _todosLocales.Select(t => new TodoDto {
            Tarea = t.Title,
            Estado = t.Completed ? "Completada ✅" : "Pendiente ⏳"
        });
        return Ok(dtos);
    }

    // Auxiliar para ver una tarea local por ID
    [HttpGet("locales/{id}")]
    public IActionResult GetLocalTodoById(int id)
    {
        var t = _todosLocales.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound("Tarea local no encontrada.");

        var dto = new TodoDto { 
            Tarea = t.Title, 
            Estado = t.Completed ? "Completada ✅" : "Pendiente ⏳" 
        };
        return Ok(dto);
    }

    // 3. UPDATE: Cambiar estado o título de una tarea local
    [HttpPut("{id}")]
    public IActionResult UpdateLocalTodo(int id, [FromBody] Todo todoActualizado)
    {
        var existente = _todosLocales.FirstOrDefault(x => x.Id == id);
        if (existente == null) return NotFound();

        existente.Title = todoActualizado.Title;
        existente.Completed = todoActualizado.Completed;

        return NoContent();
    }

    // 4. DELETE: Borrar tarea local
    [HttpDelete("{id}")]
    public IActionResult DeleteLocalTodo(int id)
    {
        var todo = _todosLocales.FirstOrDefault(x => x.Id == id);
        if (todo == null) return NotFound();

        _todosLocales.Remove(todo);
        return Ok(new { message = $"Tarea {id} eliminada de la lista local." });
    }
}