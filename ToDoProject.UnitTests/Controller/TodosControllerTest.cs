using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ToDoProject.Controllers;
using ToDoProject.Models;
using Xunit;

namespace ToDoProject.UnitTests.Controller
{
    public class TodosControllerTest
    {
        [Fact]
        public async Task Create_RedirectsToCreate_WhenModelStateIsNotValid()
        {
            var mockLogger = new Mock<ILogger<TodosController>>();
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using var context = new TodoDbContext(options);
            var controller = new TodosController(mockLogger.Object, context);
            controller.ModelState.AddModelError("Title", "Title is required");

            var result = await controller.Create(new CreateTodoModel());

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Create", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_CreatesTodoItemAndRedirectsToIndex()
        {
            var mockLogger = new Mock<ILogger<TodosController>>();
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using var context = new TodoDbContext(options);
            var controller = new TodosController(mockLogger.Object, context);
            var newTodoModel = new CreateTodoModel { Title = "Test Todo", IsCompleted = false, DueDate = DateTime.Now };

            var result = await controller.Create(newTodoModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            Assert.Single(context.Todos);
        }

        [Fact]
        public async Task Update_UpdatesTodoItemAndRedirectsToIndex()
        {
            var mockLogger = new Mock<ILogger<TodosController>>();
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using var context = new TodoDbContext(options);
            var controller = new TodosController(mockLogger.Object, context);
            var todoIdToUpdate = 5;
            var todoToUpdate = new Todo { Id = todoIdToUpdate, Title = "Test Todo", IsCompleted = false, DueDate = DateTime.Now };
            context.Todos.Add(todoToUpdate);
            context.SaveChanges();

            context.Entry(todoToUpdate).State = EntityState.Detached;

            var updatedTodoModel = new Todo { Id = todoIdToUpdate, Title = "Hello Todo", IsCompleted = true, DueDate = DateTime.Now.AddDays(1) };

            var result = await controller.Update(todoIdToUpdate, updatedTodoModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var todoInDb = await context.Todos.FindAsync(todoIdToUpdate);
            Assert.NotNull(todoInDb);
            Assert.Equal(updatedTodoModel.Title, todoInDb.Title);
            Assert.Equal(updatedTodoModel.IsCompleted, todoInDb.IsCompleted);
            Assert.Equal(updatedTodoModel.DueDate, todoInDb.DueDate);
        }

        [Fact]
        public async Task Delete_RemovesTodoItemAndRedirectsToIndex()
        {
            var mockLogger = new Mock<ILogger<TodosController>>();
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using var context = new TodoDbContext(options);
            var controller = new TodosController(mockLogger.Object, context);
            var todoIdToDelete = 1;
            var todoToDelete = new Todo { Id = todoIdToDelete, Title = "Todo Denemesi", IsCompleted = false, DueDate = DateTime.Now };

            var existingTodo = await context.Todos.FindAsync(todoIdToDelete);
            if (existingTodo == null)
            {
                context.Todos.Add(todoToDelete);
                context.SaveChanges();
            }

            var result = controller.Delete(todoIdToDelete);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var deletedTodo = await context.Todos.FindAsync(todoIdToDelete);
            Assert.Null(deletedTodo);
        }
    }
}
