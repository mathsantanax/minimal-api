using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.DB;


#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServicos, AdministradorServico>();
builder.Services.AddScoped<IVeiculosServico, VeiculosServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContesto>( options =>{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlString")
    );
});

#endregion

#region Home
var app = builder.Build();

app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

#endregion

#region Administradores
app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) => {
    if(administradorServicos.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso!");
    else
        return  Results.Unauthorized();
}).WithTags("Administradores");

#endregion

#region Veiculos
    app.MapPost("/Veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) => {
        var veiculo = new Veiculo{
            Nome =  veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Ano = veiculoDTO.Ano
        };

        veiculosServico.Incluir(veiculo);
        
        return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

    app.MapGet("/Veiculos", ([FromQuery] int? pagina, IVeiculosServico veiculosServico) => {
        var veiculo = veiculosServico.Todos(pagina);
        
        return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapGet("/Veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) => {
        var veiculo = veiculosServico.BuscaPorId(id);
        if(veiculo == null) return Results.NotFound();

        return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapPut("/Veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) => {
        var veiculo = veiculosServico.BuscaPorId(id);
        if(veiculo == null) return Results.NotFound();
        
        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;
        veiculo.Ano = veiculoDTO.Ano;

        veiculosServico.Atualizar(veiculo);
        return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapDelete("/Veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) => {
        var veiculo = veiculosServico.BuscaPorId(id);
        if(veiculo == null) return Results.NotFound();

        veiculosServico.Apagar(veiculo);
        return Results.Ok("Veiculo apagado com sucesso! ");
}).WithTags("Veiculos");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
#endregion


