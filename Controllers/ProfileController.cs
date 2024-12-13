using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniqloMvc.Extensions;
using UniqloMvc.Models;
using UniqloMvc.ViewModels.Auths;
using static System.Net.WebRequestMethods;

namespace UniqloMvc.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;
    public ProfileController(UserManager<User> userManager, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        User? user = await _userManager.FindByIdAsync(userId);

        if (user == null) return NotFound();

        ProfileUpdateVM vm = new ProfileUpdateVM
        {
            Fullname = user.Fullname,
            Username = user.UserName,
        };

        string imgUrl = user.ProfileImageUrl != null ? "../imgs/profiles/" + user.ProfileImageUrl : "https://www.pngall.com/wp-content/uploads/5/User-Profile-PNG.png";
        ViewBag.ProfileUrl = imgUrl;


        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileUpdateVM vm)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        User? user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        string path = Path.Combine(_env.WebRootPath, "imgs", "profiles");
        string fileUrl = await vm.ProfilePhoto.Upload(path);

        user.ProfileImageUrl = fileUrl;
        await _userManager.UpdateAsync(user);
        return RedirectToAction(nameof(Index));
    }
}
