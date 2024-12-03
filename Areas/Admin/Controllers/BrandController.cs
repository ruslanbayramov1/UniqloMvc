using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Enums;
using UniqloMvc.Extensions;
using UniqloMvc.Models;
using UniqloMvc.ViewModels.Brands;

namespace UniqloMvc.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(Roles.Admin))]
public class BrandController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
{
    public async Task<IActionResult> Index()
    { 
        return View(await _context.Brands.Where(brand => brand.IsDeleted == false).ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BrandCreateVM vm)
    {
        if (vm.Logo != null)
        { 
            if (!vm.Logo.IsValidType("image"))
                ModelState.AddModelError("Logo", "File must be image type");

            else if (!vm.Logo.IsValidSize(2 * 1024))
                ModelState.AddModelError("Logo", "File must be less than 2mb");
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        Brand brand = new Brand
        {
            Name = vm.Name,
            CreatedTime = DateTime.Now,
            IsDeleted = false,
        };

        if (vm.Logo != null)
        { 
            string filePath = Path.Combine(_env.WebRootPath, "imgs", "brands");
            string newFileName = await vm.Logo!.Upload(filePath);
            brand.LogoUrl = newFileName;
        }

        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return BadRequest();

        Brand? brand = await _context.Brands.Where(brand => brand.IsDeleted == false).FirstOrDefaultAsync(brand => brand.Id == id);
        if (brand == null) return NotFound();

        
        brand.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        Brand? brand = await _context.Brands.Where(brand => brand.IsDeleted == false).FirstOrDefaultAsync(brand => brand.Id == id);
        if (brand == null) return NotFound();

        ViewBag.Brand = brand;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, BrandUpdateVM vm)
    {
        if (vm.Logo != null)
        {
            if (!vm.Logo.IsValidType("image"))
                ModelState.AddModelError("Logo", "Logo must be image type");

            else if (!vm.Logo.IsValidSize(2 * 1024))
                ModelState.AddModelError("Logo", "Logo must be less than 2mb");
        }

        if (!ModelState.IsValid)
        { 
            ViewBag.Brand = await _context.Brands.Where(brand => brand.IsDeleted == false).FirstOrDefaultAsync(brand => brand.Id == id);
            return View(vm);
        }

        Brand? brand = await _context.Brands.Where(brand => brand.IsDeleted == false).FirstOrDefaultAsync(brand => brand.Id == id);
        if (brand == null) return NotFound();

        ViewBag.Brand = brand;

        if (vm.Logo != null)
        { 
            string filePath = Path.Combine(_env.WebRootPath, "imgs", "brands");
            string newFileName = await vm.Logo.Upload(filePath, brand.LogoUrl);
            brand.LogoUrl = newFileName;
        }

        brand.Name = vm.Name;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
