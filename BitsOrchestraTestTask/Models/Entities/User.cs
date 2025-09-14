using System.ComponentModel.DataAnnotations;

namespace BitsOrchestraTestTask.Models.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public string DisplayName { get; set; }

    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}