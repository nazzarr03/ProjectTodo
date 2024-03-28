using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoProject.Models;

namespace ToDoProject.Controllers;


public class TodosController : Controller
{
    
    private readonly ILogger<TodosController> _logger;
    private readonly TodoDbContext _context;
    
    public TodosController(ILogger<TodosController> logger, TodoDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public IActionResult Index()
    {
        return View(GetAllTodos());
    }
    public IActionResult Create()
    {
        
        var viewModel = new CreateTodoViewModel()
        {
            Todo = new Todo(),
            Categories = GetAllCategories()
        };
        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(CreateTodoModel todo)
    {
        Console.WriteLine(todo.Category);
        if (ModelState.IsValid)
        {
            var category = _context.Categories.Find(todo.Category);
            if (category == null)
            {
                ModelState.AddModelError("Category", "Invalid category selected.");
            }

            Todo newTodo = new Todo
            {
                Title = todo.Title,
                DueDate = todo.DueDate,
                IsCompleted = false,
                Category = category
            };

            _context.Todos.Add(newTodo);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"{todo.Title}: This named todo successfully created.");

            return RedirectToAction("Index");
        }
        
        return RedirectToAction("Create");
    }
    

    
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var todo = _context.Todos.Find(id);
        if (todo == null)
        {
            _logger.LogError($"Received null item.");
            return BadRequest();
        }

        _context.Todos.Remove(todo);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Update(int id)
    {
        var todo = _context.Todos.Find(id);
        if (todo == null)
        {
            _logger.LogError($"Received null item.");
            return RedirectToAction("Index");
        }
        return View(todo);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, Todo todo)
    {
        if (id != todo.Id)
        {
            return BadRequest();
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(todo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(todo.Id))
                {
                    _logger.LogError($"Received null item.");
                    return NotFound();
                }
            }
            _logger.LogInformation( $" {todo.Title}:This named todo successfully updated.");

            return RedirectToAction("Index");
        }

        return View(todo);
    }
    private List<Todo> GetAllTodos()
    { 
        return _context.Todos.ToList();
    }
    public List<Category> GetAllCategories()
    {
        return _context.Categories.ToList();
    }

    private bool TodoExists(int id)
    {
        return _context.Todos.Any(e => e.Id == id);
    }
    
}