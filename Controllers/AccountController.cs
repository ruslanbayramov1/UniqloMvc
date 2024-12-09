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
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Home");
        }

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

    public IActionResult VerifyResetPassword()
    {
        string? userName = HttpContext.Request.Cookies["user"];
        string? token = HttpContext.Request.Cookies["token"];
        if (userName == null || token == null) return NotFound();

        return View();
    }

    public IActionResult SendForgotEmail()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyResetPassword(ForgotVM vm)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Passwords not matches");
            return View();
        }

        string? userName = HttpContext.Request.Cookies["user"];
        string? token = HttpContext.Request.Cookies["token"];
        token = token.Replace(' ', '+');

        User? user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            ModelState.AddModelError("", "Something went wrong with resetting password");
            return View();
        };

        var res = await _userManager.ResetPasswordAsync(user, token, vm.Password);
        if (!res.Succeeded)
        {
            foreach (var err in res.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View();
        }

        HttpContext.Response.Cookies.Delete("token");
        HttpContext.Response.Cookies.Delete("user");

        await _signInManager.SignInAsync(user, true);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> SendForgotEmail(ForgotEmailVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        User? user = await _userManager.FindByEmailAsync(vm.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Something went wrong!");
            return View();
        }

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendForgotPasswordAsync(user.Email, user.UserName, token);
        return Content("Reset password email sent!");
    }

    public IActionResult ResetPasswordData(string token, string user)
    {
        CookieOptions opt = new CookieOptions
        {
            Expires = DateTime.UtcNow + TimeSpan.FromMinutes(5),
        };

        HttpContext.Response.Cookies.Append("token", token, opt);
        HttpContext.Response.Cookies.Append("user", user, opt);

        return RedirectToAction(nameof(VerifyResetPassword));
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
