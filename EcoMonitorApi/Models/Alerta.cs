namespace EcoMonitorApi.Models;

public class Alerta
{
    public int Id { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public bool Resolvido { get; set; }
}