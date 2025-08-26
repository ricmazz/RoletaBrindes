using Microsoft.AspNetCore.Mvc;
using RoletaBrindes.Application.DTOs;
using RoletaBrindes.Application.Services;

namespace RoletaBrindes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpinsController(SpinService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SpinResponse>> Spin([FromBody] SpinRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Phone))
            return BadRequest("Informe nome e telefone");

        var res = await service.SpinAsync(req.Name, req.Phone);
        return Ok(res);
    }
}