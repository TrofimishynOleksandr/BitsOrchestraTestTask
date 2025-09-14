using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitsOrchestraTestTask.Models.Entities;

public class Contact
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public bool Married { get; set; }

    [Phone]
    public string Phone { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Salary { get; set; }

    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; }
}