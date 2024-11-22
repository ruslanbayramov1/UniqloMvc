using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Extensions;
using UniqloMvc.Models;
using UniqloMvc.ViewModels.Products;

namespace UniqloMvc.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
{
    public async Task<IActionResult> Index()
    { 
        return View(await _context.Products.Where(prod => prod.IsDeleted == false).Include(x => x.Brand).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Brands = await _context.Brands.Where(brand => brand.IsDeleted == false).ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        
        if (vm.File != null)
        {
            if (!vm.File.IsValidType("image"))
                ModelState.AddModelError("File", "File type must be an image");

            else if (!vm.File.IsValidSize(2 * 1024))
                ModelState.AddModelError("File", "File type must be less than 2mb");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Brands = await _context.Brands.Where(brand => brand.IsDeleted == false).ToListAsync();
            return View(vm);
        }

        ViewBag.Brands = await _context.Brands.Where(brand => brand.IsDeleted == false).ToListAsync();
        Product product = new Product
        {
            Name = vm.Name,
            Description = vm.Description,
            SellPrice = vm.SellPrice,
            CostPrice = vm.CostPrice,
            Quantity = vm.Quantity,
            BrandId = vm.BrandId,
            CreatedTime = DateTime.Now,
            Discount = vm.Discount,
            IsDeleted = false,
        };

        if (vm.File != null)
        {
            string filePath = Path.Combine(_env.WebRootPath, "imgs", "products");
            string newFileName = vm.File.Upload(filePath).Result;
            product.CoverImage = newFileName;
        }

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        Product? product = await _context.Products.Where(brand => brand.IsDeleted == false).FirstOrDefaultAsync(prod => prod.Id == id);
        if (product == null) return NotFound();

        ViewBag.Brands = await _context.Brands.Where(brand => brand.IsDeleted == false).ToListAsync();
        ViewBag.Product = product;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, ProductUpdateVM vm)
    {
        Product? product = await _context.Products.Where(brand => brand.IsDeleted == false).FirstOrDefaultAsync(prod => prod.Id == id);
        if (product == null) return NotFound();

        if (vm.File != null)
        {
            if (!vm.File.IsValidType("image"))
                ModelState.AddModelError("File", "File must be an image");

            if (!vm.File.IsValidSize(2 * 1024))
                ModelState.AddModelError("File", "File must be less than 2mb");
        }

        ViewBag.Brands = await _context.Brands.Where(brand => brand.IsDeleted == false).ToListAsync();
        ViewBag.Product = await _context.Products.Where(brand => brand.IsDeleted == false).FirstOrDefaultAsync(prod => prod.Id == id);
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        if (vm.File != null)
        { 
            string filePath = Path.Combine(_env.WebRootPath, "imgs", "products");
            string newFileName = vm.File!.Upload(filePath).Result;
            product.CoverImage = newFileName;
        }

        product.Name = vm.Name;
        product.Description = vm.Description;
        product.SellPrice = vm.SellPrice;
        product.CostPrice = vm.CostPrice;
        product.Quantity = vm.Quantity;
        product.Name = vm.Name;
        product.BrandId = vm.BrandId;
        product.Discount = vm.Discount;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        Product? product = await _context.Products.Where(brand => brand.IsDeleted == false).FirstOrDefaultAsync(prod => prod.Id == id);
        if (product == null) return NotFound();

        product.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
