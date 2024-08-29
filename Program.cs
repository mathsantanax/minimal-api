using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enuns;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.DB;


#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option => {
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option => {
    option.TokenValidationParameters = new TokenValidationParameters{
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

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

string GerarTokenJwt(Administrador administrador){
    if(string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var Claims = new List<Claim>(){
        new Claim(ClaimTypes.Email, administrador.Email),
        new Claim("Perfil", administrador.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: Claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("Administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) => {
    var adm = administradorServicos.Login(loginDTO);
    if(adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok( new AdministradorLogado{
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return  Results.Unauthorized();
}).RequireAuthorization().WithTags("Administradores");

app.MapPost("/Administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServicos administradorServicos) => {

        var validacao = new ErrosDeValidacao{
            Mensagens = new List<string>()
        };

        if(string.IsNullOrEmpty(administradorDTO.Email))
            validacao.Mensagens.Add("Email não pode ser vazio");
        if(string.IsNullOrEmpty(administradorDTO.Senha))
            validacao.Mensagens.Add("Email não pode ser vazia");
        if(administradorDTO.Perfil == null)
            validacao.Mensagens.Add("Email não pode ser vazio");

        if(validacao.Mensagens.Count > 0)
            return Results.BadRequest(validacao);
        var administrador = new Administrador{
            Email =  administradorDTO.Email,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
        };

        administradorServicos.Incluir(administrador);
        
        return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).RequireAuthorization().WithTags("Administradores");

app.MapGet("/Administradores",([FromQuery] int? id, IAdministradorServicos administradorServicos) => {
    var administrador = new List<AdministradorModelView>();
    var administradores = administradorServicos.Todos(id);

    foreach(var adm in administradores)
    {
        administrador.Add(new AdministradorModelView{
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    if(administrador == null) return Results.NotFound(); // se não encontrar nada retorn not found

    return Results.Ok(administrador);
}).RequireAuthorization().WithTags("Administradores");

app.MapGet("/Administradores/{id}",([FromRoute] int? id, IAdministradorServicos administradorServicos) => {
    var administrador = administradorServicos.BuscarPorId(id);


    if(administrador == null) return Results.NotFound();

    return Results.Ok(new AdministradorModelView{
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
}).RequireAuthorization().WithTags("Administradores");

#endregion

#region Veiculos
    ErrosDeValidacao validaDTO (VeiculoDTO veiculoDTO)
    {
        var validacao = new ErrosDeValidacao{
            Mensagens = new List<string>()
        };

        if(string.IsNullOrEmpty(veiculoDTO.Nome))
            validacao.Mensagens.Add("O nome não pode ser vazio");

        if(string.IsNullOrEmpty(veiculoDTO.Marca))
            validacao.Mensagens.Add("O nome não pode ficar em branco");

        if(veiculoDTO.Ano < 1950)
            validacao.Mensagens.Add("Veiculo muito antigo, aceito somente anos superiores a 1950");

        return validacao;
    }

    app.MapPost("/Veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) => {
    
        var validacao = validaDTO(veiculoDTO);
        if(validacao.Mensagens.Count > 0)
            return Results.BadRequest(validacao);

        var veiculo = new Veiculo{
            Nome =  veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Ano = veiculoDTO.Ano
        };

        veiculosServico.Incluir(veiculo);
        
        return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).RequireAuthorization().WithTags("Veiculos");

    app.MapGet("/Veiculos", ([FromQuery] int? pagina, IVeiculosServico veiculosServico) => {
        var veiculo = veiculosServico.Todos(pagina);
        
        return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/Veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) => {
        var veiculo = veiculosServico.BuscaPorId(id);
        if(veiculo == null) return Results.NotFound();

        return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veiculos");

app.MapPut("/Veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) => {
        var veiculo = veiculosServico.BuscaPorId(id);
        if(veiculo == null) return Results.NotFound();

        var validacao = validaDTO(veiculoDTO);
        if(validacao.Mensagens.Count > 0)
            return Results.BadRequest(validacao);
        
        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;
        veiculo.Ano = veiculoDTO.Ano;

        veiculosServico.Atualizar(veiculo);
        return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veiculos");

app.MapDelete("/Veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) => {
        var veiculo = veiculosServico.BuscaPorId(id);
        if(veiculo == null) return Results.NotFound();

        veiculosServico.Apagar(veiculo);
        return Results.NoContent();
}).RequireAuthorization().WithTags("Veiculos");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion


