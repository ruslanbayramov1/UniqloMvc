using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Models;
using UniqloMvc.ViewModels.Brands;
using UniqloMvc.ViewModels.Commons;

namespace UniqloMvc.Controllers;

public class HomeController(UniqloDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        ICollection<Slider> sliders = await _context.Sliders.Where(slider => !slider.IsDeleted).ToListAsync();
        ICollection<BrandHomeItemVM> brands = await _context.Brands
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Products.Count)
            .Take(3)
            .Select(x => new BrandHomeItemVM { 
            Id = x.Id,
            Name = x.Name,
        }).ToListAsync();
        ICollection<Product> products = await _context.Products
            .Where(x => !x.IsDeleted)
            .Where(x => brands.Select(y => y.Id).ToList().Contains(x.BrandId!.Value))
            .Take(10)
            .ToListAsync();
        HomeVM vm = new HomeVM();
        vm.Products = products;
        vm.Sliders = sliders;
        vm.Brands = brands;

        return View(vm);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
