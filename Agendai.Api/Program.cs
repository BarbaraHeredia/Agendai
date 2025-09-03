using Agendai.Api.Data;
using Agendai.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DOS SERVIÇOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// --- ENDPOINTS DA API (CRUD Completo) ---

app.MapGet("/", () => "Bem-vindo ao Agendai API!");

// [R]ead All - Listar todos os lembretes
app.MapGet("/lembretes", async (AppDbContext db) =>
{
    var lembretes = await db.Lembretes.ToListAsync();
    return Results.Ok(lembretes);
});

// [C]reate - Criar um novo lembrete
app.MapPost("/lembretes", async (AppDbContext db, Lembrete lembrete) =>
{
    db.Lembretes.Add(lembrete);
    await db.SaveChangesAsync();
    return Results.Created($"/lembretes/{lembrete.Id}", lembrete);
});

// NOVO ENDPOINT: [R]ead One - Buscar um lembrete por ID
app.MapGet("/lembretes/{id:int}", async (AppDbContext db, int id) => 
{
    // 1. Parâmetro de Rota:
    //    O '{id:int}' na rota diz ao ASP.NET para esperar um número inteiro na URL.
    //    Esse número é automaticamente passado para a variável 'id' da nossa função.

    // 2. Buscando no Banco:
    //    'db.Lembretes.FindAsync(id)' é um método otimizado do EF Core
    //    para buscar um item pela sua chave primária. É muito eficiente.
    var lembrete = await db.Lembretes.FindAsync(id);

    // 3. Tratamento de "Não Encontrado":
    //    Se 'FindAsync' não encontrar o lembrete, ele retorna 'null'.
    //    É uma boa prática verificar isso e retornar um 404 Not Found.
    if (lembrete is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(lembrete);
});

// NOVO ENDPOINT: [U]pdate - Atualizar um lembrete existente
app.MapPut("/lembretes/{id:int}", async (AppDbContext db, int id, Lembrete lembreteAtualizado) =>
{
    var lembreteExistente = await db.Lembretes.FindAsync(id);
    if (lembreteExistente is null)
    {
        return Results.NotFound();
    }

    // Atualizamos as propriedades do objeto que encontramos no banco
    // com os novos valores que recebemos na requisição.
    lembreteExistente.Titulo = lembreteAtualizado.Titulo;
    lembreteExistente.Descricao = lembreteAtualizado.Descricao;
    lembreteExistente.Data = lembreteAtualizado.Data;
    lembreteExistente.Concluido = lembreteAtualizado.Concluido;

    // Salvamos as alterações no banco de dados.
    await db.SaveChangesAsync();

    // Retornamos 204 No Content, o status padrão para uma atualização bem-sucedida.
    return Results.NoContent();
});

// NOVO ENDPOINT: [D]elete - Deletar um lembrete
app.MapDelete("/lembretes/{id:int}", async (AppDbContext db, int id) =>
{
    var lembreteParaDeletar = await db.Lembretes.FindAsync(id);
    if (lembreteParaDeletar is null)
    {
        return Results.NotFound();
    }

    // 'db.Lembretes.Remove(...)' prepara a operação de DELETE.
    db.Lembretes.Remove(lembreteParaDeletar);

    // E 'SaveChangesAsync()' executa a operação no banco.
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

