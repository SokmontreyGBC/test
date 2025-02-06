
namespace Assignment1.Models;

public class User
{
    public User(int userId, UserType userType)
    {
        UserId = userId;
        UserType = userType;
    }

    public int UserId { get; set; }

    public UserType UserType { get; set; }
}