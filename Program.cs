var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS (Antes del Build)
builder.Services.AddControllers(); // ¡IMPORTANTE para que funcionen tus Controllers!
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Aquí registramos el cliente HTTP
builder.Services.AddHttpClient("JsonPlaceholderApi", client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

// 2. CONSTRUCCIÓN DE LA APP
var app = builder.Build();

// 3. CONFIGURACIÓN DEL PIPELINE (Después del Build)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); // Aunque no tengas login, es buena práctica tenerlo

// 4. MAPEO DE RUTAS
app.MapControllers(); // Esto hace que el PostsController y UsersController funcionen

app.Run();