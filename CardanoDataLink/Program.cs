using CardanoDataLink.Domain.DataHandler;
using CardanoDataLink.Domain.Gleif;
using CardanoDataLink.Infra.Gleif;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddScoped<IGleifClientInterface, HttPGleifRepository>();
builder.Services.AddScoped<IDataEnricher, DataEnricher>();

var app = builder.Build();

// http only for now
// app.UseHttpsRedirection();
app.MapControllers();

app.Run();
