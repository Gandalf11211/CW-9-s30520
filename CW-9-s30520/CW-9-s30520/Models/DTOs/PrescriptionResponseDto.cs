using System.ComponentModel.DataAnnotations;

namespace CW_9_s30520.Models.DTOs;

public class PrescriptionResponseDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public DoctorGetDto Doctor { get; set; } = null!;
    public List<MedicamentGetDto> Medicaments { get; set; } = null!;
}