using OfficeOpenXml;
using RaporAsistani.Data;
using RaporAsistani.Services;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.License.SetNonCommercialOrganization("none");

builder.Services.AddHttpClient<LlamaService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(100);
});

builder.Services.AddHttpClient<EmbeddingService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(100);
});

builder.Services.AddScoped<QdrantService>();
builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<PythonService>();

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<PromptService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
        builder.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
});


builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
