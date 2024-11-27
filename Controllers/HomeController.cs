using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Models;
using UniqloMvc.ViewModels.Brands;
using UniqloMvc.ViewModels.Commons;
using UniqloMvc.ViewModels.Products;

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

    public async Task<IActionResult> Shop(string? amount, string? sortby,params int?[] SelectedBrand)
    {
        var query = _context.Products
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (amount != null)
        {
            IEnumerable<int> prices = amount.Split('-').Select(x => Convert.ToInt32(x));
            query = query.Where(x => x.SellPrice - (x.SellPrice * x.Discount / 100) >= prices.ElementAt(0) && x.SellPrice - (x.SellPrice * x.Discount / 100) <= prices.ElementAt(1));
        }

        if (SelectedBrand.Length != 0)
        {
            IEnumerable<string> selectedBrands = SelectedBrand.Select(x => x.HasValue ? x.Value.ToString() : "");

            int length = SelectedBrand.Length;
            if (!selectedBrands.Contains("0"))
            {
                query = query.Where(x => selectedBrands.Contains(x.BrandId.ToString()));
            }
        }

        if (sortby != null)
        {
            if (sortby == "newness")
            {
                query = query.OrderByDescending(x => x.CreatedTime);
            }
            else if (sortby == "oldness")
            {
                query = query.OrderBy(x => x.CreatedTime);
            }
        }

        ShopVM vm = new ShopVM();

        var brands = await _context.Products.Where(x => !x.IsDeleted).ToListAsync();
        ViewBag.AllCount = brands.Count;

        vm.Brands = _context.Brands
            .Where(x => !x.IsDeleted)
            .Select(x => new BrandShopItemVM
            {
                Id = x.Id,
                Name = x.Name,
                Count = x.Products == null ? 0 : x.Products.Where(y => !y.IsDeleted).ToList().Count
            });

        vm.Products = await query
            .Select(x => new ProductShopItemVM
            {
                Id = x.Id,
                Name = x.Name,
                SellPrice = x.SellPrice,
                Discount = x.Discount,
                ImageUrl = x.CoverImage
            }).ToListAsync();
        return View(vm);
    }

    //[HttpPost]
    //public async Task<IActionResult> Shop(string? amount, params int?[] SelectedBrand)
    //{
    //    var query = _context.Products
    //        .Where(x => !x.IsDeleted)
    //        .AsQueryable();

    //    if (amount != null)
    //    {
    //        IEnumerable<int> prices = amount.Split('-').Select(x => Convert.ToInt32(x));
    //        query.Where(x => x.SellPrice >= prices.ElementAt(0) && x.SellPrice <= prices.ElementAt(1));
    //    }

    //    if (SelectedBrand.Length != 0)
    //    {
    //        IEnumerable<string> brands = SelectedBrand.Select(x => x.HasValue ? x.Value.ToString() : "");

    //        int length = SelectedBrand.Length;
    //        query.Where(x => brands.Contains(x.BrandId.ToString()));
    //    }

    //    return View(query);
    //}
}
