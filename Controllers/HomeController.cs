using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Models;

namespace UniqloMvc.Controllers;

public class HomeController(UniqloDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        List<Slider> sliders = await _context.Sliders.Where(slider => slider.IsDeleted == false).ToListAsync();
        ViewBag.Products = await _context.Products.Where(prod => prod.IsDeleted == false).ToListAsync();
        return View(sliders);
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
