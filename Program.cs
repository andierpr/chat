using AIChatRailway.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// HttpClient para IA
builder.Services.AddHttpClient<OpenAIService>();

// CORS (libera frontend React)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware
app.UseCors("AllowAll");

// Controllers
app.MapControllers();

// Endpoint raiz (evita 404 confuso)
app.MapGet("/", () => "AI Chat API rodando 🚀");

app.Run();