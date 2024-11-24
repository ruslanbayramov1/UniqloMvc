using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Models;
using UniqloMvc.Views.Commons;

namespace UniqloMvc.Controllers;

public class HomeController(UniqloDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        ICollection<Slider> sliders = await _context.Sliders.Where(slider => slider.IsDeleted == false).ToListAsync();
        ICollection<Product> products = await _context.Products.Where(prod => prod.IsDeleted == false).ToListAsync();
        HomeVM vm = new HomeVM();
        vm.Products = products;
        vm.Sliders = sliders;

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
}
