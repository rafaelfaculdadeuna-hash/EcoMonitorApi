using Microsoft.EntityFrameworkCore;
using EcoMonitorApi.Models;

namespace EcoMonitorApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Dispositivo> Dispositivos => Set<Dispositivo>();
    public DbSet<Sensor> Sensores => Set<Sensor>();
    public DbSet<Leitura> Leituras => Set<Leitura>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
}