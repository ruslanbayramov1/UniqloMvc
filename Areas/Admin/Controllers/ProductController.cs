using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.Constants;
using UniqloMvc.DataAccess;
using UniqloMvc.Enums;
using UniqloMvc.Extensions;
using UniqloMvc.Models;
using UniqloMvc.ViewModels.Products;

namespace UniqloMvc.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AuthTypeCustom.AdminAndSmm)]
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
                ModelState.AddModelError("File", "File size must be less than 2mb");
        }

        if (vm.OtherFiles != null && vm.OtherFiles.Any())
        {
            if (!vm.OtherFiles.All(x => x.IsValidType("image")))
            {
                List<string> fileNames = vm.OtherFiles.Where(x => !x.IsValidType(ContentType.ImageType)).Select(x => x.FileName).ToList();
                ModelState.AddModelError("OtherFiles", $"Invalid type of files: {String.Join(", ", fileNames)}. Must be an image");
            }
            if (!vm.OtherFiles.All(x => x.IsValidSize(ContentType.ImageSize)))
            {
                List<string> fileNames = vm.OtherFiles.Where(x => !x.IsValidSize(ContentType.ImageSize)).Select(x => x.FileName).ToList();
                ModelState.AddModelError("OtherFiles", $"Invalid size of files: {String.Join(", ", fileNames)}. Must be less than 2mb");
            }
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

        if (vm.OtherFiles != null)
        { 
            string filePath = Path.Combine(_env.WebRootPath, "imgs", "products");
            product.Images = vm.OtherFiles.Select(x => new ProductImage
            {
                Product = product,
                ImageUrl = x.Upload(filePath).Result,
            }).ToList(); 
        }

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        Product? product = await _context.Products
            .Include(prod => prod.Images)
            .Where(prod => prod.IsDeleted == false)
            .FirstOrDefaultAsync(prod => prod.Id == id);

        if (product == null) return NotFound();

        ProductUpdateVM vm = product;
        ViewBag.Brands = await _context.Brands.ToListAsync();

        return View(vm);
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

        if (vm.OtherFiles != null && vm.OtherFiles.Any())
        {
            if (!vm.OtherFiles.All(x => x.IsValidType("image")))
            {
                List<string> fileNames = vm.OtherFiles.Where(x => !x.IsValidType(ContentType.ImageType)).Select(x => x.FileName).ToList();
                ModelState.AddModelError("OtherFiles", $"Invalid type of files: {String.Join(", ", fileNames)}. Must be an image");
            }
            if (!vm.OtherFiles.All(x => x.IsValidSize(ContentType.ImageSize)))
            {
                List<string> fileNames = vm.OtherFiles.Where(x => !x.IsValidSize(ContentType.ImageSize)).Select(x => x.FileName).ToList();
                ModelState.AddModelError("OtherFiles", $"Invalid size of files: {String.Join(", ", fileNames)}. Must be less than 2mb");
            }
        }

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

        if (vm.OtherFiles != null && vm.OtherFiles.Any())
        {
            string filePath = Path.Combine(_env.WebRootPath, "imgs", "products");
            product.Images = vm.OtherFiles.Select(x => new ProductImage
            {
                Product = product,
                ImageUrl = x.Upload(filePath).Result,
            }).ToList();
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
        await _context.ProductImages.Where(x => x.ProductId == id).ForEachAsync(x => x.IsDeleted = true);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteImgs(int id, IEnumerable<string> imgNames)
    {
        int res = await _context.ProductImages.Where(x => imgNames.Contains(x.ImageUrl)).ExecuteDeleteAsync();

        if (res >= 1)
        {
            string fullPath;
            foreach (string img in imgNames)
            {
                fullPath = Path.Combine(_env.WebRootPath, "imgs", "products", img);
                System.IO.File.Delete(img);
            }
        }

        return RedirectToAction(nameof(Update), new { id });
    }
}
