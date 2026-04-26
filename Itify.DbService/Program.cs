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
    var usersDb = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    scope.ServiceProvider.GetRequiredService<EquipmentDbContext>().Database.Migrate();
    usersDb.Database.Migrate();

    if (!usersDb.Users.Any())
    {
        byte[] salt = [0xAF, 0xA5, 0xB5, 0x46, 0xD1, 0xA7, 0xB6, 0xB8, 0xFD, 0xA1, 0xB2, 0x37, 0xFA, 0xF1, 0x32, 0x46];
        var hash = Convert.ToBase64String(Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivation.Pbkdf2(
            "default", salt, Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA256, 1000, 256 / 8));
        usersDb.Users.Add(new Itify.DbService.Entities.User
        {
            Name = "Admin",
            Email = "admin@default.com",
            Password = hash,
            Role = Itify.DbService.Enums.UserRoleEnum.Admin
        });
        usersDb.SaveChanges();
    }
}

app.MapControllers();
app.Run();
