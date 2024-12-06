using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using UniqloMvc.Enums;
using UniqloMvc.Extensions;
using UniqloMvc.Models;
using UniqloMvc.Services.Abstracts;
using UniqloMvc.ViewModels.Auths;

namespace UniqloMvc.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    public async Task<IActionResult> VerifyEmail(string token, string user)
    {
        User? userEnt = await _userManager.FindByNameAsync(user);
        if (userEnt == null) return BadRequest();

        token = token.Replace(' ', '+');
        var res = await _userManager.ConfirmEmailAsync(userEnt, token);
        if (!res.Succeeded)
        {
            foreach (var err in res.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
        }

        await _signInManager.SignInAsync(userEnt, true);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        User user = new User
        {
            Fullname = vm.Fullname,
            Email = vm.EmailAddress,
            UserName = vm.Username,
        };

        var res = await _userManager.CreateAsync(user, vm.Password);
        if (!res.Succeeded)
        {
            foreach (var err in res.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View();
        }

        var roleRes = await _userManager.AddToRoleAsync(user, Roles.User.GetRole());
        if (!roleRes.Succeeded)
        {
            foreach (var err in roleRes.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View();
        }

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailConfirmationAsync(user.Email, user.UserName, token);
        return Content("Email sent!");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm, string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        { 
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            return View();
        }


        User? user = await _userManager.FindByNameAsync(vm.Username);
        if (user == null)
        {
            ModelState.AddModelError("", "Username or password is wrong");
            return View();
        }

        if (!user.EmailConfirmed)
        {
            ModelState.AddModelError("", "You need to confirm you email!");
            return View();
        }

        var res = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);

        if (!res.Succeeded)
        {
            if (res.IsNotAllowed)
            {
                ModelState.AddModelError("", "You do not have permission");
            }
            else if (res.IsLockedOut)
            {
                ModelState.AddModelError("", $"Wait until {user.LockoutEnd!.Value}");
            }
            else
            {
                ModelState.AddModelError("", "Username or password is wrong");
            }
            return View();
        }
        
        if (!returnUrl.IsNullOrEmpty())
        { 
            return Redirect(returnUrl!);
        }

        if (HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value.ToLower() == "admin")
        {
            return RedirectToAction("Index", new { Controller="Dashboard", Area="Admin"});
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}
