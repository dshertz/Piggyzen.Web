using MediatR;
using Microsoft.AspNetCore.Mvc;
using Piggyzen.Web.Features.Tag;
using Index = Piggyzen.Web.Features.Tag.Index;

namespace Piggyzen.Web.Controllers
{
    [Route("[controller]")]
    public class TagController : Controller
    {
        private readonly IMediator _mediator;

        public TagController(IMediator mediator) => _mediator = mediator;

        // GET: /Tag
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var tags = await _mediator.Send(new Index.Query());
            return View(tags);
        }

        // GET: /Tag/Details/{id}
        /* [HttpGet("{id}")]
        public async Task<IActionResult> Details(Details.Query query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        } */
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _mediator.Send(new Details.Query { Id = id });
            return View(model);
        }

        // GET: /Tag/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Tag/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Create.Command command)
        {
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tag/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Edit.Query query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        }

        // POST: /Tag/Edit
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed(Edit.Command command)
        {
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tag/Delete/{id}
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(Delete.Query query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        }

        // POST: /Tag/Delete
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Delete.Command command)
        {
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
    }
}