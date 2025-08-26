using Microsoft.AspNetCore.Mvc;
using RoletaBrindes.Application.DTOs;
using RoletaBrindes.Domain.Models;
using RoletaBrindes.Infrastructure.Data;
using RoletaBrindes.Infrastructure.Repositories.Interfaces;

namespace RoletaBrindes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GiftsController(IGiftRepository repo, IConnectionFactory f) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Gift>>> Get()
    {
        using var conn = f.NewConnection(); await conn.OpenAsync();
        // Sem FOR UPDATE (só leitura)
        var gifts = await repo.ListActiveAsync();
        return Ok(gifts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GiftIn input)
    {
        using var conn = f.NewConnection(); await conn.OpenAsync();
        var id = await repo.CreateAsync(new Gift { Name = input.Name, Stock = input.Stock, Weight = input.Weight, IsActive = true });
        return Created($"/api/gifts/{id}", new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] GiftIn input)
    {
        using var conn = f.NewConnection(); await conn.OpenAsync();
        var rows = await repo.UpdateAsync(new Gift { Id = id, Name = input.Name, Stock = input.Stock, Weight = input.Weight });
        return rows == 0 ? NotFound() : Ok(new { updated = rows });
    }
}