using System.Globalization;
using BitsOrchestraTestTask.Data;
using BitsOrchestraTestTask.Helpers;
using BitsOrchestraTestTask.Models.Dtos;
using BitsOrchestraTestTask.Models.Entities;
using CsvHelper;
using CsvHelper.Configuration;

namespace BitsOrchestraTestTask.Services;

public class CsvImportService : ICsvImportService
{
    private readonly ApplicationDbContext _context;
    private readonly IContactValidator _validator;

    public CsvImportService(ApplicationDbContext context, IContactValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<CsvImportResult> ImportContactsAsync(IFormFile file, Guid userId)
    {
        var result = new CsvImportResult();
        if (file == null || file.Length == 0)
        {
            result.Errors.Add("Empty file.");
            return result;
        }

        using var stream = new StreamReader(file.OpenReadStream());
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            PrepareHeaderForMatch = args => args.Header?.ToLower().Replace(" ", ""),
            BadDataFound = null,
            MissingFieldFound = null
        };

        using var csv = new CsvReader(stream, config);
        if (!csv.Read() || !csv.ReadHeader())
        {
            result.Errors.Add("CSV has no header.");
            return result;
        }

        int rowNumber = 1;
        var validContacts = new List<Contact>();

        while (csv.Read())
        {
            rowNumber++;

            try
            {
                var name = csv.GetField("Name");
                var dobText = csv.GetField("DateOfBirth");
                var marriedText = csv.GetField("Married");
                var phone = csv.GetField("Phone");
                var salaryText = csv.GetField("Salary");

                if (!DateTime.TryParse(dobText, out var dob) ||
                    !decimal.TryParse(salaryText, NumberStyles.Number, CultureInfo.InvariantCulture, out var salary) ||
                    !bool.TryParse(marriedText, out var married))
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Row {rowNumber}: parsing error.");
                    continue;
                }

                var contact = new Contact
                {
                    Name = name,
                    DateOfBirth = dob,
                    Married = married,
                    Phone = phone,
                    Salary = salary,
                    UserId = userId
                };

                var contactValidationResult = _validator.Validate(contact);
                if (!contactValidationResult.IsValid)
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Row {rowNumber}: {string.Join(", ", contactValidationResult.Errors)}");
                    continue;
                }

                validContacts.Add(contact);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.ErrorCount++;
                result.Errors.Add($"Row {rowNumber}: {ex.Message}");
            }
        }

        if (validContacts.Any())
        {
            _context.Contacts.AddRange(validContacts);
            await _context.SaveChangesAsync();
        }

        return result;
    }
}