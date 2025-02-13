using EventSignupApi.Context;
using EventSignupApi.Services;
using EventSignupApi.Services.ENV;
using EventSignupApi.Services.LevenShteinService;
//Load env files.

await DotEnv.Load("./.env");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Servicene her har ingen state knyttet direkte til seg, de representerer en samling metoder som skal kjøres på en eller flere modeller. De kan derfor implementeres som en transient service.
//dvs vi lager og disposer en instans hver gang vi kaller den. 
builder.Services.AddTransient<EventDtoService>();
builder.Services.AddTransient<EventDataHandler>();
builder.Services.AddTransient<UserDtoService>();
builder.Services.AddTransient<UserHandler>();
builder.Services.AddTransient<Ls>();

//TokenService skal holde oversikt over aktive sessions, denne staten skal helst holdes i livet over hele livssyklussen til appen vår, og bør derfor implementeres som en singleton.
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
