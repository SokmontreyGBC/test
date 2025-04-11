using Assignment1.Controllers;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Assignment1.Data;

[TestClass]
public class UnitTest
{
    [TestMethod]
    public void Create_ValidProduct_ReturnsRedirectToAdminIndex()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestProductDb")
            .Options;

        var context = new ApplicationDbContext(options);

        var logger = new Mock<ILogger<ProductController>>().Object;

        var controller = new ProductController(context, logger);

        var product = new Product
        {
            ProductName = "Test Product",
            ProductDescription = "Description",
            ProductPrice = 19.99m,
            ProductStock = 10,
            CategoryId = 1
        };
        
        var result = controller.Create(product);
        
        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        var redirectResult = (RedirectToActionResult)result;
        Assert.AreEqual("Index", redirectResult.ActionName);
        Assert.AreEqual("Admin", redirectResult.ControllerName);

        Assert.AreEqual(1, context.Products.CountAsync().Result);
    }
}