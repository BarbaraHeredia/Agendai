namespace Agendai.Api.Models;

public class Lembrete
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime Data { get; set; }
    public bool Concluido { get; set; }
}