using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(); // To enable CORS

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddSwaggerGen(options => 
{
    options.SwaggerDoc("v1", new OpenApiInfo 
    {
        Version = "v1",
        Title = "FirstAPI",
        Description = "My first api",
        TermsOfService = new Uri("https://www.google.com"),
    });

    // The token is applied on all request
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
    });

});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Prefered to be added before the use authorization middleware
// the allow any origin is not a very good security practice
app.UseCors(
    cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
); 


app.UseAuthorization();

app.MapControllers();

app.Run();
