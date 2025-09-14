namespace BitsOrchestraTestTask.Models.Dtos;

public class ContactValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}