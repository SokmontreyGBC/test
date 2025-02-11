
namespace Assignment1.Models;

public class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public UserType UserType { get; set; }

    // navigation properties

    public ICollection<Cart>? Carts { get; set; } = new List<Cart>();
}