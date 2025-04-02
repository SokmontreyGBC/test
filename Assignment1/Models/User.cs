
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Assignment1.Models;

public class User : IdentityUser
{
    [Key]
    public int UserId { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [Display(Name = "Email")]
    public string UserEmail { get; set; }
    [Required]
    [Display(Name = "Full Name")]
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public UserType UserType { get; set; }
}