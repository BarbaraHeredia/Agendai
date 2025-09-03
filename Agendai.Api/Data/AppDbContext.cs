using Agendai.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Agendai.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Lembrete> Lembretes { get; set; }
}