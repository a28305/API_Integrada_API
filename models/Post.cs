namespace MiApiIntegrada.Models;

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; } // La clave que conecta con el usuario
    public string Title { get; set; }
    public string Body { get; set; }
}