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
        var products = await _context.Products
         .ToListAsync();

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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(Product product)
    {
        var dbProduct = await _context.Products.FindAsync(product.Id);
        // EF начинает отслеживать объект dbProduct, поэтому _context.Products.Update(dbProduct) не нужно 

        if (dbProduct == null)
            return NotFound();

        dbProduct.Name = product.Name;
        dbProduct.Price = product.Price;
        dbProduct.AvailableQuantity = product.AvailableQuantity;
        if (product.Image != null)
        {
            var fileName = dbProduct.ImageFileName;
            fileName??=Guid.NewGuid() +
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

            dbProduct.ImageFileName = fileName;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(product.ImageFileName))
        {
            var filePath = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "products",
                product.ImageFileName);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }            

        return RedirectToAction(nameof(Index));
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
                if (product.AvailableQuantity <= 0)
                    return RedirectToAction(nameof(Index));

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
                if (item.Quantity >= product.AvailableQuantity)
                    return RedirectToAction(nameof(Index));

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