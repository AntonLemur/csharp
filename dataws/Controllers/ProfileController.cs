using System.Linq;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Index()
    {
        // 1. Получаем текущего пользователя
        // User — это текущий пользователь из cookie
        var user = await _userManager.GetUserAsync(User);

        // 2. Получаем Customer
        // var customer = _context.Customers
        //     .FirstOrDefault(c => c.UserId == user.Id);

        return View(user);//customer);
    }
}