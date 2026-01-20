namespace MiApiIntegrada.Models.Dto;

// Para recibir datos nuevos (Entrada)
public class UserCreateDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
}