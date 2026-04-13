using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoMonitorApi.Data;
using EcoMonitorApi.Models;

namespace EcoMonitorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertasController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // Listar todos os alertas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Alerta>>> Get()
        => await _context.Alertas.OrderByDescending(a => a.DataEmissao).ToListAsync();

    // Criar um novo alerta
    [HttpPost]
    public async Task<ActionResult<Alerta>> Post(Alerta alerta)
    {
        _context.Alertas.Add(alerta);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = alerta.Id }, alerta);
    }

    // Buscar alerta por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Alerta>> GetById(int id)
    {
        var alerta = await _context.Alertas.FindAsync(id);
        return alerta == null ? NotFound() : alerta;
    }

    // --- MÉTODOS DE ANÁLISE E OPERAÇÃO ---

    // 1. Filtrar apenas alertas não resolvidos (Pendentes)
    [HttpGet("pendentes")]
    public async Task<ActionResult<IEnumerable<Alerta>>> GetPendentes()
    {
        return await _context.Alertas
            .Where(a => !a.Resolvido)
            .ToListAsync();
    }

    // 2. Marcar um alerta como resolvido (Ação rápida)
    [HttpPatch("{id}/resolver")]
    public async Task<IActionResult> ResolverAlerta(int id)
    {
        var alerta = await _context.Alertas.FindAsync(id);
        if (alerta == null) return NotFound();

        alerta.Resolvido = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Alerta {id} marcado como resolvido." });
    }

    // 3. Análise: Percentual de alertas resolvidos vs pendentes
    [HttpGet("dashboard/metricas")]
    public async Task<ActionResult> GetMetricas()
    {
        var total = await _context.Alertas.CountAsync();
        var resolvidos = await _context.Alertas.CountAsync(a => a.Resolvido);
        var pendentes = total - resolvidos;

        // Evita divisão por zero
        double taxaResolucao = total > 0 ? (double)resolvidos / total * 100 : 0;

        return Ok(new
        {
            TotalAlertas = total,
            Resolvidos = resolvidos,
            Pendentes = pendentes,
            TaxaResolucao = $"{taxaResolucao:F2}%",
            UltimaAtualizacao = DateTime.Now
        });
    }
}