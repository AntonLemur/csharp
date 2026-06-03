using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// [Authorize]
public class ProductsController : Controller
{
    private readonly DataContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(DataContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();

        return View(products);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (product.Image != null)
        {
            var fileName =
                Guid.NewGuid() +
                Path.GetExtension(product.Image.FileName);

            string uploadsFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "products");

            Directory.CreateDirectory(
                uploadsFolder);

            string filePath =
                Path.Combine(
                    uploadsFolder,
                    fileName);

            using var stream =
                new FileStream(filePath, FileMode.Create);

            await product.Image.CopyToAsync(stream);

            product.ImageFileName = fileName;
        }

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeQuantity(int id, bool plus)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

        var item = cart.FirstOrDefault(x => x.ProductId == id);

        if(plus)
        {
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }
        }
        else if (item != null)
        {
            item.Quantity--;

            if (item.Quantity <= 0)
            {
                cart.Remove(item);
            }
        }

        HttpContext.Session.SetObject("Cart", cart);

        return RedirectToAction("Index");
    }
}