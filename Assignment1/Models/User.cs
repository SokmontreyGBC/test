
namespace Assignment1.Models;

public class User
{
    public int UserId { get; set; }
    public string UserEmail { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public UserType UserType { get; set; }
}