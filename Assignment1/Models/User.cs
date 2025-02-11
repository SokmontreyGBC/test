namespace Assignment1.Models;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } // Blank for guest
    public string? UserPassword { get; set; } // Blank for guest
    public string UserEmail { get; set; }
    public UserType UserType { get; set; }
}