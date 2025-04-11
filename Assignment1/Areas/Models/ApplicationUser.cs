using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Assignment1.Areas.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(20, ErrorMessage = "First name cannot be longer than 20 characters.")]
    public string FirstName { get; set; }
    [Required]
    [StringLength(20, ErrorMessage = "Last name cannot be longer than 20 characters.")]
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
