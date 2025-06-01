namespace CW_9_s30520.Models.DTOs;

public class MedicamentGetDto
{
    public int MedicamentId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Dose { get; set; }
    public string Details { get; set; } = null!;
}