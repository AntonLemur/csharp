using System.Linq;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;

    public ProfileController(UserManager<ApplicationUser> userManager, DataContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [Authorize] //пускает только залогиненных
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // 1. Получаем текущего пользователя
        // User — это текущий пользователь из cookie
        var user = await _userManager.GetUserAsync(User);

        // 2. Получаем Customer
        var customer = _context.Customers
            .FirstOrDefault(c => c.UserId == user.Id);

        // Чтобы кабинет открывался даже у пользователей,
        // созданных до появления таблицы Customer.
        if (customer == null)
        {
            customer = new Customer
            {
                UserId = user.Id,
                Email = user.Email
            };

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();
        }
                    
        return View(customer);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Index(Customer model)
    {
        var user = await _userManager.GetUserAsync(User);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        customer.FirstName = model.FirstName;
        customer.LastName = model.LastName;
        customer.MiddleName = model.MiddleName;
        customer.Address = model.Address;
        customer.Email = model.Email;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}