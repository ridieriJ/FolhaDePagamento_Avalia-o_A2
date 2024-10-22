using ApiFolha.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

//POST api/funcionario/cadastrar
app.MapPost("/api/funcionario/cadastrar", async ([FromBody] Funcionario funcionario, [FromServices] AppDbContext contextFuncionarios) =>
{
    contextFuncionarios.Funcionarios.Add(funcionario);
    await contextFuncionarios.SaveChangesAsync();

    return Results.Created("", funcionario);
});

//GET	api/funcionario/listar
app.MapGet("/api/funcionario/listar", async ([FromServices] AppDbContext contextFuncionarios) =>
{
    var funcionarios = await contextFuncionarios.Funcionarios.ToListAsync();
    if (funcionarios.Any())
    {
        return Results.Ok(funcionarios);
    }
    return Results.NotFound("Nenhum funcionário registrado");
});

//POST	api/folha/cadastrar
app.MapPost("/api/folha/cadastrar", async ([FromBody] Folha folha, [FromServices] AppDbContext contextFolha) =>
{
    contextFolha.Folhas.Add(folha);
    await contextFolha.SaveChangesAsync();

    return Results.Created("", folha);
});

//GET	api/folha/listar
app.MapGet("/api/folha/listar", async ([FromServices] AppDbContext contextFolha) =>
{
    var folhas = await contextFolha.Folhas
        .Include(f => f.Funcionario) 
        .ToListAsync();

    if (folhas.Any())
    {
        return Results.Ok(folhas);
    }
    return Results.NotFound("Nenhuma folha registrada");
});

//GET api/folha/buscar/{cpf}/{mes}/{ano}
app.MapGet("/api/folha/buscar/{cpf}/{mes}/{ano}", async (string cpf, int mes, int ano, [FromServices] AppDbContext contextFolha) =>
{
    var funcionario = await contextFolha.Funcionarios.FirstOrDefaultAsync(f => f.Cpf == cpf);

    if (funcionario == null)
    {
        return Results.NotFound("Funcionário não encontrado");
    }

    var folha = await contextFolha.Folhas
                    .Where(f => f.FuncionarioId == funcionario.Id && f.Mes == mes && f.Ano == ano)
                    .FirstOrDefaultAsync();

    if (folha == null)
    {
        return Results.NotFound("Folha de pagamento não encontrada para este período");
    }

    var salarioBruto = folha.Valor * folha.Quantidade;

    var inss = CalcularINSS(salarioBruto);

    var ir = CalcularIR(salarioBruto);

    var fgts = salarioBruto * 0.08m;

    var salarioLiquido = salarioBruto - inss - ir;

    var resultado = new
    {
        NomeFuncionario = funcionario.Nome,
        SalarioBruto = salarioBruto,
        INSS = inss,
        IR = ir,
        FGTS = fgts,
        SalarioLiquido = salarioLiquido
    };

    return Results.Ok(resultado);
});

//calculo do INSS
decimal CalcularINSS(decimal salarioBruto)
{
    if (salarioBruto <= 1693.72m)
        return salarioBruto * 0.08m;
    else if (salarioBruto <= 2822.90m)
        return salarioBruto * 0.09m;
    else if (salarioBruto <= 5645.80m)
        return salarioBruto * 0.11m;
    else
        return 621.03m;
}

// calculo do IR
decimal CalcularIR(decimal salarioBruto)
{
    if (salarioBruto <= 1903.98m)
        return 0m;
    else if (salarioBruto <= 2826.65m)
        return salarioBruto * 0.075m - 142.80m;
    else if (salarioBruto <= 3751.05m)
        return salarioBruto * 0.15m - 354.80m;
    else if (salarioBruto <= 4664.68m)
        return salarioBruto * 0.225m - 636.13m;
    else
        return salarioBruto * 0.275m - 869.36m;
}

app.Run();