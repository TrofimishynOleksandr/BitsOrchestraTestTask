namespace BitsOrchestraTestTask.Models.Dtos;

public class CsvImportResult
{
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
}