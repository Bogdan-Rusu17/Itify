using Itify.DbService.Contexts;
using Itify.DbService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Itify DB Service", Version = "v1" });
});

var usersConn = builder.Configuration.GetConnectionString("UsersDb")!;
var equipmentConn = builder.Configuration.GetConnectionString("EquipmentDb")!;

builder.Services.AddDbContext<UsersDbContext>(o => o.UseNpgsql(usersConn));
builder.Services.AddDbContext<EquipmentDbContext>(o => o.UseNpgsql(equipmentConn));

builder.Services.AddScoped<IRepository<UsersDbContext>, Repository<UsersDbContext>>();
builder.Services.AddScoped<IRepository<EquipmentDbContext>, Repository<EquipmentDbContext>>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Itify DB Service v1"));

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<UsersDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<EquipmentDbContext>().Database.Migrate();
}

app.MapControllers();
app.Run();
