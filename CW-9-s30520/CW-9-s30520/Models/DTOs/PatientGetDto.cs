using System.ComponentModel.DataAnnotations;

namespace CW_9_s30520.Models.DTOs;

public class PatientGetDto
{
    public int? IdPatient { get; set; }
    
    [MaxLength(100)]
    public required string FirstName { get; set; } = null!;
    
    [MaxLength(100)]
    public required string LastName { get; set; } = null!;
    
    public required DateTime BirthDate { get; set; }
}