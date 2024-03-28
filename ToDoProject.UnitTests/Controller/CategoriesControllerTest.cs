using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ToDoProject.Controllers;
using ToDoProject.Models;
using Xunit;

namespace ToDoProject.Tests.Controllers
{
    public class CategoriesControllerTests : IDisposable
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly TodoDbContext _context;

        public CategoriesControllerTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();
            _logger = factory.CreateLogger<CategoriesController>();

            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TodoDbContext(options);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async void Create_ValidCategory_ReturnsRedirectToActionResult()
        {
            var controller = new CategoriesController(_logger, _context);
            var newCategory = new Category { Id = 1, Name = "TestCategory" };

            var result = await controller.Create(newCategory) as Microsoft.AspNetCore.Mvc.RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async void Update_InvalidId_ReturnsNotFoundResult()
        {
            var controller = new CategoriesController(_logger, _context);
            var invalidCategory = new Category { Id = 999, Name = "InvalidCategory" };

            var result = await controller.Update(invalidCategory.Id, invalidCategory);

            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingCategory_ReturnsRedirectToActionResult()
        {
            var controller = new CategoriesController(_logger, _context);
            var categoryToDelete = new Category { Id = 1, Name = "TestCategory" };
            await _context.Categories.AddAsync(categoryToDelete);
            await _context.SaveChangesAsync();

            var result = controller.Delete(categoryToDelete.Id);

            var redirectToActionResult = Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
        
        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenCategoryNotFound()
        {
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using var context = new TodoDbContext(options);
            var controller = new CategoriesController(mockLogger.Object, context);

            var result = controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
