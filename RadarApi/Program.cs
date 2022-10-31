using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RadarApi.DbContexts;
using System.Text.Json.Serialization;
using RadarApi.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "myPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<RadarContext>();

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        var converter = new JsonStringEnumConverter();
        options.JsonSerializerOptions.Converters.Add(converter);
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(FileUploadUtil.uploadDirectory)),
    RequestPath = FileUploadUtil.requestPath
});

app.UseCors();


app.UseAuthorization();

app.MapControllers();

app.Run();
