using RagBasics.EmbeddingGenerator;
using RagBasics.Repository;
using RagBasics.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("PostgreSQL");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Required configuration settings are missing.");
}

// Register services
builder.Services.AddSingleton<IEmbeddingGenerator>(sp =>
    new OllamaEmbeddingGenerator(new Uri("http://127.0.0.1:11434"), "mistral"));

builder.Services.AddSingleton(sp =>
    new TextRepository(connectionString, sp.GetRequiredService<IEmbeddingGenerator>()));

builder.Services.AddSingleton(sp =>
    new RagService(sp.GetRequiredService<TextRepository>(), new Uri("http://127.0.0.1:11434"), "mistral"));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
