var builder = WebApplication.CreateBuilder(args);

// Añadir servicios al contenedor.
builder.Services.AddControllers();

// Configuración de CORS (opcional)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS (opcional)
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.MapControllers(); // Esto asegura que se mapeen todos los controladores en la aplicación

app.Run();
