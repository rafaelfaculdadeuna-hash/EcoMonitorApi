using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoMonitorApi.Data;
using EcoMonitorApi.Models;

namespace EcoMonitorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeiturasController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // Listar todas as leituras (com limite de segurança para performance)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Leitura>>> Get([FromQuery] int limite = 100)
        => await _context.Leituras
            .OrderByDescending(l => l.DataHora)
            .Take(limite)
            .ToListAsync();

    // Registrar uma nova leitura
    [HttpPost]
    public async Task<ActionResult<Leitura>> Post(Leitura leitura)
    {
        _context.Leituras.Add(leitura);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = leitura.Id }, leitura);
    }

    // --- MÉTODOS DE ANÁLISE ---

    // 1. Obter histórico por Sensor
    [HttpGet("sensor/{sensorId}")]
    public async Task<ActionResult<IEnumerable<Leitura>>> GetBySensor(int sensorId)
    {
        return await _context.Leituras
            .Where(l => l.SensorId == sensorId)
            .OrderByDescending(l => l.DataHora)
            .ToListAsync();
    }

    // 2. Análise Estatística: Média, Máximo e Mínimo de um sensor
    [HttpGet("sensor/{sensorId}/estatisticas")]
    public async Task<ActionResult> GetEstatisticas(int sensorId)
    {
        var leituras = _context.Leituras.Where(l => l.SensorId == sensorId);

        if (!await leituras.AnyAsync())
            return NotFound(new { message = "Nenhuma leitura encontrada para este sensor." });

        var analise = new
        {
            SensorId = sensorId,
            Media = await leituras.AverageAsync(l => l.Valor),
            ValorMaximo = await leituras.MaxAsync(l => l.Valor),
            ValorMinimo = await leituras.MinAsync(l => l.Valor),
            TotalLeituras = await leituras.CountAsync(),
            UltimaLeitura = await leituras.MaxAsync(l => l.DataHora)
        };

        return Ok(analise);
    }

    // 3. Filtrar leituras acima de um limite (Alerta de criticidade)
    [HttpGet("alertas/limite")]
    public async Task<ActionResult<IEnumerable<Leitura>>> GetLeiturasAcimaDoLimite([FromQuery] double limite)
    {
        var criticas = await _context.Leituras
            .Where(l => l.Valor > limite)
            .OrderByDescending(l => l.Valor)
            .ToListAsync();

        return Ok(criticas);
    }
}