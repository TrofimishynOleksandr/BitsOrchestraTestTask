using BitsOrchestraTestTask.Models.Dtos;
using BitsOrchestraTestTask.Models.Entities;

namespace BitsOrchestraTestTask.Helpers;

public interface IContactValidator
{
    ContactValidationResult Validate(Contact contact);
    ContactValidationResult ValidateField(string field, string value);
}