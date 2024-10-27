using BookAPI;
using BookAPI.Database;
using BookAPI.Repositories;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services;
using BookAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<DateOnlySchemaFilter>(); 
});

//RegisterUnitOfWork and repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookRepository,BookRepository>();
builder.Services.AddScoped<IAuthorRepository,AuthorRepository>();
builder.Services.AddScoped<IBookAuthorRepository, BookAuthorRepository>();

//Register Services
builder.Services.AddScoped<IBookService,BookService>();
builder.Services.AddScoped<IAuthorService,AuthorService>();

//Configure Entity Framework with SQL Server
builder.Services.AddDbContext<BookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookDB")));

var app = builder.Build();

// Configure the HTTP request pipeline. [Middleware]
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
