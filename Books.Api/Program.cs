using Books.Api.Contexts;
using Books.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BooksContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("BooksDBConnectionString")));
builder.Services.AddControllers();

builder.Services.AddScoped<IBooksRepository, BooksRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
