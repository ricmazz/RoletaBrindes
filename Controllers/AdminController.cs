using Microsoft.AspNetCore.Mvc;
using RoletaBrindes.Application.DTOs;
using RoletaBrindes.Domain.Models;
using RoletaBrindes.Infrastructure.Data;
using RoletaBrindes.Infrastructure.Repositories.Interfaces;

namespace RoletaBrindes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(IGiftRepository repoGift, ISpinRepository repoSpin, IParticipantRepository repoParticipant, IConnectionFactory f) : ControllerBase
{
    [HttpGet("GetGifts")]
    public async Task<ActionResult<IEnumerable<Gift>>> GetGifts()
    {
        using var conn = f.NewConnection(); await conn.OpenAsync();
        
        var gifts = await repoGift.ListAllAsync();
        return Ok(gifts);
    }
    
    [HttpGet("GetParticipants")]
    public async Task<ActionResult<IEnumerable<Gift>>> GetParticipants()
    {
        using var conn = f.NewConnection(); await conn.OpenAsync();
        
        var gifts = await repoParticipant.ListAllAsync();
        return Ok(gifts);
    }
    
    [HttpGet("GetSpins")]
    public async Task<ActionResult<IEnumerable<Gift>>> GetSpins()
    {
        using var conn = f.NewConnection(); await conn.OpenAsync();
        
        var gifts = await repoSpin.ListAllAsync();
        return Ok(gifts);
    }
    
    [HttpDelete("DeleteSpin")]
    public async Task<ActionResult<bool>> GetSpins(int spinId)
    {
        using var conn = f.NewConnection(); await conn.OpenAsync();
        
        var spinWasDelete = await repoSpin.DeleteAsync(spinId);;

        if (spinWasDelete)
        {
            return Ok();
        }
        else
        {
            return BadRequest();       
        }
    }
}