using System.ComponentModel.DataAnnotations;

namespace CW_9_s30520.Models.DTOs;

public class PrescriptionMedicamentCreateDto
{
    public required int IdMedicament { get; set; }
    public required int Dose { get; set; }
    
    [MaxLength(100)]
    public required string Description { get; set; } = null!;
}