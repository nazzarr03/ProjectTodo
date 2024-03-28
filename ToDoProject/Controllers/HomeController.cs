using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToDoProject.Models;

namespace ToDoProject.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Todos");
    }
    
}