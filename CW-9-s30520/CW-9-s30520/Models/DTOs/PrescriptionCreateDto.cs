using System.ComponentModel.DataAnnotations;

namespace CW_9_s30520.Models.DTOs;

public class PrescriptionCreateDto
{
    public required PatientGetDto Patient { get; set; } = null!;
    
    [MinLength(1)]
    [MaxLength(10)]
    public required List<PrescriptionMedicamentCreateDto> Medicaments { get; set; } = null!;
    public required DateTime Date {get; set; }
    public required DateTime DueDate { get; set; }
    public int IdDoctor { get; set; }
}