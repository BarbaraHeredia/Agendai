using Agendai.Api.Data;
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


// --- ENDPOINTS DA API ---

app.MapGet("/", () => "Bem-vindo ao Agendai API!");

// NOVO ENDPOINT: Listar todos os lembretes
// Este é o coração da nossa API interagindo com o banco.
app.MapGet("/lembretes", async (AppDbContext db) =>
{
    // 1. Injeção de Dependência em Ação:
    //    O ASP.NET nos entrega uma instância do nosso 'AppDbContext' (que chamamos de 'db').
    //    Não precisamos nos preocupar em como criá-lo.

    // 2. Consulta com Entity Framework:
    //    'db.Lembretes' acessa a tabela de lembretes.
    //    '.ToListAsync()' é um comando do EF que se traduz para "SELECT * FROM Lembretes" em SQL.
    //    'await' é usado porque a operação de banco de dados pode demorar, e não queremos travar a aplicação.
    var lembretes = await db.Lembretes.ToListAsync();
    
    // 3. Retorno da Resposta:
    //    Retornamos um status 200 OK com a lista de lembretes em formato JSON.
    return Results.Ok(lembretes);
});


app.Run();