namespace EcoMonitorApi.Models;

public class Leitura
{
    public int Id { get; set; }
    public double Valor { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public int SensorId { get; set; }
}