namespace MiApiIntegrada.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; } // Objeto anidado
}

public class Address
{
    public string City { get; set; }
}