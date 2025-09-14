using BitsOrchestraTestTask.Models.Dtos;

namespace BitsOrchestraTestTask.Services;

public interface ICsvImportService
{
    Task<CsvImportResult> ImportContactsAsync(IFormFile file, Guid userId);
}