using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Extensions;
using UniqloMvc.Models;
using UniqloMvc.ViewModels.Sliders;

namespace UniqloMvc.Areas.Admin.Controllers;

[Area("Admin")]
public class SliderController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Sliders.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SliderCreateVM vm)
    {
        if (!vm.File.IsValidType("image"))
        {
            ModelState.AddModelError("File", "File tpye must be image");
        }
        else if (!vm.File.IsValidSize(2 * 1024))
        {
            ModelState.AddModelError("File", "File size must be less than 2mb");
        }

        if (!ModelState.IsValid) return View(vm);

        string folderLocation = Path.Combine(_env.WebRootPath, "imgs", "sliders");
        string newFileName = vm.File!.Upload(folderLocation).Result;

        Slider slider = new Slider
        {
            ImageUrl = newFileName,
            Title = vm.Title,
            Subtitle = vm.Subtitle,
            Link = vm.Link,
            CreatedTime = DateTime.Now,
            IsDeleted = false,
        };

        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        Slider? slider = await _context.Sliders.FirstOrDefaultAsync(slider => slider.Id == id);
        if (slider == null) return NotFound();

        ViewBag.Slider = slider;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, SliderUpdateVM vm)
    {
        Slider? slider = await _context.Sliders.FirstOrDefaultAsync(slider => slider.Id == id);
        if (slider == null) return NotFound();

        if (vm.File != null)
        {
            if (vm.File != null && !vm.File.IsValidType("image"))
            {
                ModelState.AddModelError("File", "File tpye must be image");
            }
            else if (vm.File != null && !vm.File.IsValidSize(2 * 1024))
            {
                ModelState.AddModelError("File", "File size must be less than 2mb");
            }
        }

        ViewBag.Slider = slider;
        if (!ModelState.IsValid)
        {
            return View(vm);
        };

        slider.Subtitle = vm.Subtitle;
        slider.Title = vm.Title;
        slider.Link = vm.Link;
        if (vm.File != null)
        {
            slider.ImageUrl = vm.File.Upload(Path.Combine(_env.WebRootPath, "imgs", "sliders"), slider.ImageUrl).Result;
        }
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        Slider? slider = await _context.Sliders.FirstOrDefaultAsync(slider => slider.Id == id);
        if (slider == null) return NotFound();

        string pathFile = Path.Combine(_env.WebRootPath, "imgs", "sliders", slider.ImageUrl);

        if (!System.IO.File.Exists(pathFile)) return NotFound();

        System.IO.File.Delete(pathFile);

        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Hide(int? id)
    {
        Slider? slider = await _context.Sliders.FirstOrDefaultAsync(slider => slider.Id == id);
        if (slider == null) return NotFound();

        slider.IsDeleted = slider.IsDeleted ? false : true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
