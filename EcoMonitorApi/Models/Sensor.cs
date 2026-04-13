namespace EcoMonitorApi.Models;

public class Sensor
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // Ex: Temperatura, Umidade
    public int DispositivoId { get; set; }
}