using EventSignupApi.Context;
using EventSignupApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<DTOService>();
builder.Services.AddTransient<EventDataHandler>();
builder.Services.AddTransient<UserDtoService>();
builder.Services.AddTransient<UserHandler>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddLogging(builder => builder.AddConsole());
/* Her lager vi en Service som skal kunne levere en context av databasen vår som kan injectes til controllerene våre senere. */
builder.Services.AddDbContext<DatabaseContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseDefaultFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
