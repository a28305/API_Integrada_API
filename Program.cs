var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS
// Importante: AddControllers permite que la API encuentre PostsController y TodosController automáticamente
builder.Services.AddControllers(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuramos el cliente HTTP (esta configuración sirve para Users, Posts y Todos)
builder.Services.AddHttpClient("JsonPlaceholderApi", client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

var app = builder.Build();

// 2. CONFIGURACIÓN DEL PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 3. MAPEO DE RUTAS
// Esto escanea todos tus controladores y crea los endpoints en Swagger
app.MapControllers(); 

app.Run();