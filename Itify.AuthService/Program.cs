using Itify.AuthService.HttpClients;
using Itify.AuthService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "Itify Auth Service", Version = "v1" }));

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtService>();

builder.Services.AddHttpClient<IDbServiceClient, DbServiceClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["DbService:BaseUrl"]!));

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Itify Auth Service v1"));

app.MapControllers();
app.Run();
