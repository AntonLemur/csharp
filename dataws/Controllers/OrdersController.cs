using System;
using System.Linq;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class OrdersController : Controller
{
    private readonly DataContext _context;

    public OrdersController(DataContext context)
    {
        _context = context;
    }

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

    // GET: Выводим форму создания
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
}