using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoMonitorApi.Data;
using EcoMonitorApi.Models;

namespace EcoMonitorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DispositivosController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Dispositivo>>> Get()
        => await _context.Dispositivos.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Dispositivo>> Post(Dispositivo dispositivo)
    {
        _context.Dispositivos.Add(dispositivo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dispositivo.Id }, dispositivo);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Dispositivo>> GetById(int id)
    {
        var dispositivo = await _context.Dispositivos.FindAsync(id);

        if (dispositivo == null)
        {
            return NotFound(new { message = "Dispositivo não encontrado." });
        }

        return dispositivo;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dispositivo = await _context.Dispositivos.FindAsync(id);

        if (dispositivo == null)
        {
            return NotFound(new { message = "Dispositivo não encontrado para exclusão." });
        }

        _context.Dispositivos.Remove(dispositivo);
        await _context.SaveChangesAsync();

        return NoContent(); // Resposta padrão 204 para deleção bem-sucedida
    }

    // 1. Filtra dispositivos por localização (Query String)
    // Exemplo: api/dispositivos/buscar?localizacao=Laboratorio
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<Dispositivo>>> GetByLocation([FromQuery] string localizacao)
    {
        var resultados = await _context.Dispositivos
            .Where(d => d.Localizacao.Contains(localizacao))
            .ToListAsync();

        return Ok(resultados);
    }

    // 2. Retorna estatísticas simplificadas (Análise de Dashboard)
    // Retorna o total de dispositivos e a lista de locais únicos monitorados
    [HttpGet("dashboard/resumo")]
    public async Task<ActionResult> GetResumo()
    {
        var total = await _context.Dispositivos.CountAsync();

        var locaisUnicos = await _context.Dispositivos
            .Select(d => d.Localizacao)
            .Distinct()
            .ToListAsync();

        return Ok(new
        {
            TotalDispositivos = total,
            QuantidadeLocais = locaisUnicos.Count,
            LocaisMonitorados = locaisUnicos,
            DataConsulta = DateTime.Now
        });
    }
}