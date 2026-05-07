using System;
using System.Linq;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class CustomersController : Controller
{
    private readonly DataContext _context;

    public CustomersController(DataContext context)
    {
        _context=context;
    }

    // Список клиентов
    [HttpGet("customers")]
        public async Task<IActionResult> Customers()
        {
            var list = await _context.Customers.ToListAsync();
            return View(list);
        }

        // Сохранение клиента
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(Customer customer)        
        {
            try 
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Customers));
            }
            catch (Exception ex)
            {
                // 1. Логируем ошибку для себя
                // _logger.LogError(ex, "Ошибка сохранения клиента");

                // 2. Выводим понятное сообщение пользователю в форме
                ModelState.AddModelError("", "Не удалось сохранить данные. Попробуйте позже.");

                // 3. Возвращаем ту же вьюху, чтобы пользователь не потерял введенные данные
                return View(customer); 
            }            
        }        
}