using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

// --------------------------------------------------------------------------------
// 1. Configuração de Serviços (Dependency Injection - DI)
// --------------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);

// 1.1 Configuração do Entity Framework Core e SQLite (Persistência)
// Usei o SQLite e configurei a Connection String lida do appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=APIUsuarios.db")
);

// 1.2 Configuração da Injeção de Dependência (DI) - Padrões de Projeto (Requisito 4)
// Usei a Injeção de Dependência para registrar os padrões Repository e Service como Scoped, 
// seguindo o princípio IoC (Inversão de Controle).
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>(); // Implementação do Repository Pattern
builder.Services.AddScoped<IUsuarioService, UsuarioService>();       // Implementação do Service Pattern (Lógica de Negócio)

// 1.3 Configuração do FluentValidation (Validação de Entrada - Requisito 1.3/4.4)
// Usei o FluentValidation para registrar os validadores. Isso garante a validação dos DTOs antes de entrar no Service.
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>(); // Registra meus validadores

// 1.4 Configuração do Swagger/OpenAPI (Documentação da API - Requisito 5)
// Adicionei o OpenAPI para documentação e testes da API.
builder.Services.AddOpenApi();

// --------------------------------------------------------------------------------
// 2. Criação do Aplicativo e Pipeline
// --------------------------------------------------------------------------------
var app = builder.Build();

// 3. Configuração do Pipeline de Requisição HTTP
if (app.Environment.IsDevelopment())
{
    // Em desenvolvimento, habilitei a interface do Swagger para facilitar os testes.
    app.MapOpenApi();
    
    // Auto-aplicação de Migrations (Code First): Fiz a aplicação automática no startup 
    // para garantir que o banco esteja sempre atualizado com o modelo.
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate(); // Aplica migrations pendentes
    }
}

app.UseHttpsRedirection();

// 3.1 Tratamento Global de Exceções (Mapeamento de Status Codes - Requisito 5)
// Implementei este handler para traduzir exceções de negócio (como email duplicado) 
// diretamente para o Status Code 409 Conflict, garantindo a consistência da API.
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Mapeamento para 409 Conflict
        if (exception is ApplicationException || (exception?.Message.Contains("Email") == true && exception.Message.Contains("cadastrado") == true))
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict; // Retorna 409 Conflict
            context.Response.ContentType = Application.Json;
            await context.Response.WriteAsJsonAsync(new { status = 409, title = "Conflict", detail = exception.Message });
        }
        else
        {
            // Erros não tratados (500 Internal Server Error)
            context.Response.StatusCode = StatusCodes.Status500InternalServerError; // Retorna 500
            context.Response.ContentType = Application.Json;
            await context.Response.WriteAsJsonAsync(new { status = 500, title = "Internal Server Error", detail = "Um erro inesperado ocorreu. Contate o administrador." });
        }
    });
});

// --------------------------------------------------------------------------------
// 4. Mapeamento dos Endpoints (Minimal APIs - Operações CRUD)
// --------------------------------------------------------------------------------

var usuariosApi = app.MapGroup("/usuarios").WithTags("Usuários"); // Criei o agrupamento base /usuarios

// 4.1 GET /usuarios - Lista todos (Read All)
usuariosApi.MapGet("/", async (IUsuarioService service, CancellationToken ct) =>
{
    var result = await service.ListarAsync(ct);
    return Results.Ok(result); // 200 OK
})
.WithName("ListarUsuarios")
.Produces<IEnumerable<UsuarioReadDto>>(StatusCodes.Status200OK);

// 4.2 GET /usuarios/{id} - Busca por ID (Read One)
usuariosApi.MapGet("/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var result = await service.ObterAsync(id, ct);
    return result is null ? Results.NotFound() : Results.Ok(result); // 200 OK ou 404 Not Found
})
.WithName("ObterUsuarioPorId")
.Produces<UsuarioReadDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// 4.3 POST /usuarios - Cria novo usuário (Create)
usuariosApi.MapPost("/", async (UsuarioCreateDto dto, IUsuarioService service, IValidator<UsuarioCreateDto> validator, CancellationToken ct) =>
{
    // Validação do DTO (formato, idade, etc.) usando o FluentValidation.
    var validationResult = await validator.ValidateAsync(dto, ct);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary()); // 400 Bad Request
    }
    
    // Chamo o Service para executar a lógica de negócio e persistência.
    var createdUsuario = await service.CriarAsync(dto, ct);
    
    // Retorna 201 Created (Recurso criado com sucesso).
    return Results.Created($"/usuarios/{createdUsuario.Id}", createdUsuario);
})
.WithName("CriarUsuario")
.Produces<UsuarioReadDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict); // <--- Final do POST

// 4.4 PUT /usuarios/{id} - Atualiza completo (Update)
usuariosApi.MapPut("/{id:int}", async (int id, UsuarioUpdateDto dto, IUsuarioService service, IValidator<UsuarioUpdateDto> validator, CancellationToken ct) =>
{
    // Validação do DTO de atualização.
    var validationResult = await validator.ValidateAsync(dto, ct);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary()); // 400 Bad Request
    }

    // Chamo o Service para atualização, que inclui a checagem de e-mail exclusivo.
    var updatedUsuario = await service.AtualizarAsync(id, dto, ct);
    
    return updatedUsuario is null ? Results.NotFound() : Results.Ok(updatedUsuario); // 200 OK ou 404 Not Found
})
.WithName("AtualizarUsuario")
.Produces<UsuarioReadDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status409Conflict); 

// 4.5 DELETE /usuarios/{id} - Remove (Soft Delete - Requisito 6)
usuariosApi.MapDelete("/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    // O Service implementa o Soft Delete (apenas muda o status Ativo para false).
    var success = await service.RemoverAsync(id, ct);
    
    // Retorna 204 No Content (remoção bem-sucedida) ou 404.
    return success ? Results.NoContent() : Results.NotFound(); 
})
.WithName("RemoverUsuario")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

// --------------------------------------------------------------------------------
// 5. Execução
// --------------------------------------------------------------------------------
app.Run();

// Classe parcial necessária para o uso do 'app.MapOpenApi()' no .NET 8/9.
// Removi o 'public' para evitar conflitos de compilação com a classe implícita.
partial class Program { }