using MediatR;
using Microsoft.AspNetCore.Mvc;
using Piggyzen.Web.Features.Category;
using Index = Piggyzen.Web.Features.Category.Index;

namespace Piggyzen.Web.Controllers
{
    [Route("[controller]")]
    public class CategoryController : Controller
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")] // GET: /Category/Index
        public async Task<IActionResult> Index()
        {
            var categories = await _mediator.Send(new Index.Query());
            return View(categories);
        }

        [HttpGet("Details/{id}")] // GET: /Category/Details/{id}
        public async Task<IActionResult> Details(Details.Query query)
        {
            var category = await _mediator.Send(query);
            return View(category);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var model = await _mediator.Send(new Create.Query());
            return View(model);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Create.Command command)
        {
            if (!ModelState.IsValid)
            {
                var model = await _mediator.Send(new Create.Query());
                model.Name = command.Name;
                model.ParentCategoryId = command.ParentCategoryId;
                return View(model);
            }

            await _mediator.Send(command);
            TempData["Message"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")] // GET: /Category/Edit/{id}
        public async Task<IActionResult> Edit(Edit.Query query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        }

        [HttpPost("Edit")] // POST: /Category/Edit
        public async Task<IActionResult> EditConfirmed(Edit.Command command)
        {
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Delete/{id}")] // GET: /Category/Delete/{id}
        public async Task<IActionResult> Delete(Delete.Query query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        }

        [HttpPost("Delete")] // POST: /Category/Delete
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Delete.Command command)
        {
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
    }
}