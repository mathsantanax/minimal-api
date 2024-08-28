using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.DB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServicos, AdministradorServico>();

builder.Services.AddDbContext<DbContesto>( options =>{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlString")
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) => {
    if(administradorServicos.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso!");
    else
        return  Results.Unauthorized();
});

app.Run();

