using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CartController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;

    public CartController(UserManager<ApplicationUser> userManager, DataContext context)
    {
      _userManager = userManager;
      _context = context;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session
            .GetObject<List<CartItem>>("Cart")
            ?? new List<CartItem>();

        return View(cart);
    }

    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var cart = HttpContext.Session
            .GetObject<List<CartItem>>("Cart")
            ?? new List<CartItem>();

        if (!cart.Any())
            return RedirectToAction("Index");

        //Проверка наличия товара
        foreach (var item in cart)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null)
                return BadRequest("Товар не найден");

            if (product.AvailableQuantity < item.Quantity)
            {
                ModelState.AddModelError(
                    "",
                    $"Недостаточно товара '{product.Name}' на складе");

                return View("Index", cart);
            }
        }            

        var user = await _userManager.GetUserAsync(User);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        var order = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.Now,
            Status = OrderStatus.New,
            Amount = (float)cart.Sum(x => x.Price * x.Quantity),
            Items = new List<OrderItem>()
        };

        foreach (var item in cart)
        {
            //Уменьшение остатка
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            product.AvailableQuantity -= item.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            });
        }

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("Cart");

        return RedirectToAction("MyOrders", "Orders");
    }    
}