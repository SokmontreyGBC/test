using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

public class OrderController : Controller   
{
    private readonly ApplicationDbContext _context;
    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreateOrder(List<OrderItem> cartItems)
    {
        try
        {
            // temp guest user
            var user = new User { UserId = -1, UserEmail = "Test", UserType = UserType.Guest };
            _context.Users.Add(user);
            _context.SaveChanges();
            // create the order first for an id
            var order = new Order
            {
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.Pending,
                UserId = -1,
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                _context.OrderItems.Add(orderItem);
            }

            _context.SaveChanges();
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    
}