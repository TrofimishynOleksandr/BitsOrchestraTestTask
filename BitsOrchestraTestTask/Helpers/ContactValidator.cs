using System.Text.RegularExpressions;
using BitsOrchestraTestTask.Models.Dtos;
using BitsOrchestraTestTask.Models.Entities;

namespace BitsOrchestraTestTask.Helpers;

public class ContactValidator : IContactValidator
    {
        public ContactValidationResult Validate(Contact contact)
        {
            var result = new ContactValidationResult();

            if (string.IsNullOrWhiteSpace(contact.Name))
                result.Errors.Add("Name is required.");

            if (contact.Salary < 0)
                result.Errors.Add("Salary cannot be negative.");

            if (contact.DateOfBirth > DateTime.UtcNow)
                result.Errors.Add("Date of birth cannot be in the future.");

            if (string.IsNullOrWhiteSpace(contact.Phone))
                result.Errors.Add("Phone is required.");
            
            if (!string.IsNullOrWhiteSpace(contact.Phone) &&
                !Regex.IsMatch(contact.Phone, @"^\+?\d{7,15}$"))
                result.Errors.Add("Phone number format is invalid.");

            result.IsValid = !result.Errors.Any();
            return result;
        }

        public ContactValidationResult ValidateField(string field, string value)
        {
            var result = new ContactValidationResult();

            try
            {
                switch (field)
                {
                    case "Name":
                        if (string.IsNullOrWhiteSpace(value))
                            result.Errors.Add("Name cannot be empty.");
                        break;
                    case "Salary":
                        if (!decimal.TryParse(value, out var salary) || salary < 0)
                            result.Errors.Add("Salary must be a valid non-negative number.");
                        break;
                    case "DateOfBirth":
                        if (!DateTime.TryParse(value, out var dob) || dob > DateTime.UtcNow)
                            result.Errors.Add("Date of birth is invalid.");
                        break;
                    case "Married":
                        if (!bool.TryParse(value, out _))
                            result.Errors.Add("Married must be true/false.");
                        break;
                    case "Phone":
                        if (!Regex.IsMatch(value, @"^\+?\d{7,15}$"))
                            result.Errors.Add("Phone number format is invalid.");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Validation error: {ex.Message}");
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }
    }