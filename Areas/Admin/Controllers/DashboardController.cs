using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniqloMvc.Enums;

namespace UniqloMvc.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(Roles.Admin))]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
