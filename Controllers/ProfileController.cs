using Microsoft.AspNetCore.Mvc;

namespace UniqloMvc.Controllers;

public class ProfileController : Controller
{
    public IActionResult Index()
    { 
        
        return View();
    }
}
