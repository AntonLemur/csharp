using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class OrdersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;

    public OrdersController(UserManager<ApplicationUser> userManager, DataContext context)
    {
      _userManager = userManager;
      _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("orders")]
    public async Task<IActionResult> Orders()
    {
        try 
        {
            // Получаем данные напрямую из Order и Customer
            var ordersData = await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new {
                    o.Id,
                    o.OrderDate,
                    o.Amount,
                    o.Status,
                    CustomerName = (o.Customer.FirstName + " " + o.Customer.LastName).Trim(),
                    o.Customer.Address
                })
                .ToListAsync();

            // Список для модального окна
            ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "LastName");

            return View(ordersData);
        }
        catch (Exception ex)
        {
            // логирование
            return View("Error");
        }
    }

    //Закрыть страницы авторизацией
    [Authorize]
    public async Task<IActionResult> MyOrders()
    {
        var user = await _userManager.GetUserAsync(User);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        var orders = await _context.Orders
            .Where(o => o.CustomerId == customer.Id)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    // это для курьерской программы
    // GET: Выводим форму создания
    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("CreateOrder")]
    public async Task<IActionResult> CreateOrder()
    {
        // Загружаем список клиентов, чтобы пользователь мог выбрать одного
        var customers = await _context.Customers
            .Select(c => new { c.Id, Name = c.FirstName + " " + c.LastName })
            .ToListAsync();
        
        ViewBag.Customers = customers; // Передаем список в форму
        return View();
    }

    // POST: Сохраняем данные
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("CreateOrder")]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        try
        {
            // order.CustomerId придет из <select name="CustomerId"> в модальном окне
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); 
            return RedirectToAction("Orders");
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            return View("Error");
        }
    }

    // это для Системы управления заказами
    // [Authorize(Roles = "Admin")]
    // [HttpGet]
    // public async Task<IActionResult> Create()
    // {
    //     ViewBag.Products = new SelectList(
    //         await _context.Products.ToListAsync(),
    //         "Id",
    //         "Name");

    //     return View();
    // }

    // [Authorize(Roles = "Admin")]
    // [HttpPost]
    // public async Task<IActionResult> Create(CreateOrderViewModel model)
    // {
    //     var user = await _userManager.GetUserAsync(User);

    //     var customer = await _context.Customers
    //         .FirstOrDefaultAsync(c => c.UserId == user.Id);

    //     if (customer == null)
    //         return Unauthorized();

    //     var product = await _context.Products
    //         .FirstOrDefaultAsync(p => p.Id == model.ProductId);

    //     if (product == null)
    //         return BadRequest();

    //     var order = new Order
    //     {
    //         CustomerId = customer.Id,
    //         OrderDate = DateTime.Now,
    //         Status = 1,
    //         Amount = (float)(product.Price * model.Quantity),
    //         Items = new List<OrderItem>()
    //     };

    //     order.Items.Add(new OrderItem
    //     {
    //         ProductId = product.Id,
    //         Quantity = model.Quantity,
    //         Price = product.Price
    //     });

    //     _context.Orders.Add(order);

    //     await _context.SaveChangesAsync();

    //     return RedirectToAction("MyOrders");
    // }
}