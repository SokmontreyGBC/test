using Microsoft.AspNetCore.Identity;

namespace Assignment1.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}