using CW_9_s30520.Exceptions;
using CW_9_s30520.Models.DTOs;
using CW_9_s30520.Service;
using Microsoft.AspNetCore.Mvc;

namespace CW_9_s30520.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionController(IDbService service) : ControllerBase
{

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrescriptionById([FromRoute] int id)
    {
        try
        {
            var prescription = await service.GetPrescriptionByIdAsync(id);
            return Ok(prescription);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionCreateDto prescriptionData)
    {
        try
        {
            var prescription = await service.CreatePrescriptionAsync(prescriptionData);
            return CreatedAtAction(nameof(GetPrescriptionById), new { id = prescription.IdPrescription }, prescription);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (EmptyPrescriptionException e)
        {
            return BadRequest(e.Message);
        }
        catch (OutOfBoundException e)
        {
            return BadRequest(e.Message);
        }
    }
    
}