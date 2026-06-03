using System;
using System.IO;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class KycController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly IWebHostEnvironment _environment;

    public KycController(UserManager<ApplicationUser> userManager, DataContext context, IWebHostEnvironment environment)
    {
      _userManager = userManager;
      _context = context;
      _environment = environment;
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }    
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        KycRequestViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user =
            await _userManager.GetUserAsync(User);

        string uploadsFolder =
            Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "kyc");

        Directory.CreateDirectory(
            uploadsFolder);

        string fileName =
            Guid.NewGuid() +
            Path.GetExtension(
                model.Document.FileName);

        string filePath =
            Path.Combine(
                uploadsFolder,
                fileName);

        using (var stream =
            new FileStream(
                filePath,
                FileMode.Create))
        {
            await model.Document.CopyToAsync(
                stream);
        }

        var request = new KycRequest
        {
            UserId = user.Id,
            PassportNumber = model.PassportNumber,
            DocumentFileName = fileName,
            CreatedAt = DateTime.Now,
            Status = KycStatus.Pending
        };

        _context.KycRequests.Add(
            request);

        await _context.SaveChangesAsync();

        return RedirectToAction(
            "Index",
            "Home");
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Requests()
    {
        return View(
            await _context.KycRequests.ToListAsync());
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> Approve(long id)
    {
        var kycrequest = await _context.KycRequests
            .FirstOrDefaultAsync(c =>
                c.Id == id);

        if (kycrequest == null)
            return NotFound();

        kycrequest.Status = KycStatus.Approved; 

        await _context.SaveChangesAsync();

        var user = await _userManager
            .FindByIdAsync(kycrequest.UserId);

        if (user != null)
        {
            user.KycStatus = KycStatus.Approved;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction(nameof(Requests));
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> Reject(long id)
    {
        var kycrequest = await _context.KycRequests
            .FirstOrDefaultAsync(c =>
                c.Id == id);

         if (kycrequest == null)
            return NotFound();

        kycrequest.Status = KycStatus.Rejected; 

        await _context.SaveChangesAsync();

        var user = await _userManager
            .FindByIdAsync(kycrequest.UserId);

        if (user != null)
        {
            user.KycStatus = KycStatus.Rejected;
            await _userManager.UpdateAsync(user);
        }        

        return RedirectToAction(nameof(Requests));
    }    
}