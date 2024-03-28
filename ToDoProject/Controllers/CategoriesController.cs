using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoProject.Models;

namespace ToDoProject.Controllers;

public class CategoriesController : Controller
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly TodoDbContext _context;
    
    public CategoriesController(ILogger<CategoriesController> logger, TodoDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Index()
    {
        return View(GetAllCategories());
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<ActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Add(category);
    
            await _context.SaveChangesAsync();
            _logger.LogInformation( $" {category.Name}:This named category successfully created.");
            return RedirectToAction("Index");// burası da düzenlenebilir.
        }
        
        return RedirectToAction("Create"); // burası düzenlenecek
    }

    public async Task<ActionResult> Update(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
        {
            _logger.LogError($"Received null category.");
            return RedirectToAction("Index"); // category sayfası olacak.
        }
        return View(category);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    _logger.LogError($"Received null category.");
                    return NotFound();
                }
            }
            _logger.LogInformation( $" {category.Name}:This named category successfully updated.");

            return RedirectToAction("Index"); // category sayfasına gidecek.
        }

        return View(category);
    }
    
    public IActionResult Delete(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
        {
            _logger.LogError($"Received null category.");
            return NotFound();
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();

        return RedirectToAction("Index"); // category sayfasına gidecek.
    }

    private List<Category> GetAllCategories()
    {
        return _context.Categories.ToList();
    }
    
    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}