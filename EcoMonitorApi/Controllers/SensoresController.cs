using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoMonitorApi.Data;
using EcoMonitorApi.Models;

namespace EcoMonitorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensoresController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // Listar todos os sensores
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sensor>>> Get()
        => await _context.Sensores.ToListAsync();

    // Cadastrar um novo sensor
    [HttpPost]
    public async Task<ActionResult<Sensor>> Post(Sensor sensor)
    {
        _context.Sensores.Add(sensor);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = sensor.Id }, sensor);
    }

    // --- MÉTODOS DE ANÁLISE E RELACIONAMENTO ---

    // 1. Listar sensores de um dispositivo específico
    [HttpGet("dispositivo/{dispositivoId}")]
    public async Task<ActionResult<IEnumerable<Sensor>>> GetByDispositivo(int dispositivoId)
    {
        return await _context.Sensores
            .Where(s => s.DispositivoId == dispositivoId)
            .ToListAsync();
    }

    // 2. Agrupar sensores por tipo (Análise de Inventário)
    // Retorna quantos sensores existem de cada tipo (Ex: Temperatura: 5, Umidade: 3)
    [HttpGet("inventario/por-tipo")]
    public async Task<ActionResult> GetContagemPorTipo()
    {
        var inventario = await _context.Sensores
            .GroupBy(s => s.Tipo)
            .Select(g => new
            {
                Tipo = g.Key,
                Quantidade = g.Count()
            })
            .ToListAsync();

        return Ok(inventario);
    }

    // 3. Buscar sensores por tipo específico
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<Sensor>>> GetByTipo([FromQuery] string tipo)
    {
        var sensores = await _context.Sensores
            .Where(s => s.Tipo.ToLower() == tipo.ToLower())
            .ToListAsync();

        return Ok(sensores);
    }
}