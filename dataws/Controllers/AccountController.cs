using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly DataContext _context;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataContext context)
    {
        _userManager = userManager;
        _context = context;
        _signInManager=signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password)
    {
        // Внутри IdentityUser уже есть
        // public virtual string Id { get; set; }
        // public virtual string UserName { get; set; }
        // public virtual string Email { get; set; }
        // public virtual string PasswordHash { get; set; }
        // и ещё много чего:
        // PhoneNumber
        // EmailConfirmed
        // LockoutEnd
        // SecurityStamp
        // TwoFactorEnabled
        // и т.д.        
        // , а ApplicationUser наследуется от IdentityUser
        // Это главный смысл Identity
        // Вы:
        // НЕ создаёте пользователя с нуля,
        // а расширяете готовую систему.
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            KycStatus = "Pending"
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            var customer = new Customer
            {
                Email = email,
                UserId = user.Id
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View();
    }


    // При логине Identity автоматически пытается загрузить:
    // claims,
    // roles,
    // external logins,
    // tokens.
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }
}

