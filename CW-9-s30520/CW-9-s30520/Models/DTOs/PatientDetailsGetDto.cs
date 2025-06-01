namespace CW_9_s30520.Models.DTOs;

public class PatientDetailsGetDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public List<PrescriptionResponseDto> Prescriptions { get; set; } = null!;
}