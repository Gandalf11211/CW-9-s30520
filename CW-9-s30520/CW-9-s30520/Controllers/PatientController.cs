using CW_9_s30520.Exceptions;
using CW_9_s30520.Service;
using Microsoft.AspNetCore.Mvc;

namespace CW_9_s30520.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientController(IDbService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientDetails([FromRoute] int id)
    {
        try
        {
            var patient = await service.GetPatientDetailsByIdAsync(id);
            return Ok(patient);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}