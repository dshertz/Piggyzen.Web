using Microsoft.AspNetCore.Mvc;
using MediatR;
using Piggyzen.Web.Features.Transaction;
using Index = Piggyzen.Web.Features.Transaction.Index;
using System.Text;

namespace Piggyzen.Web.Controllers
{
    [Route("[controller]")]
    public class TransactionController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IHttpClientFactory _httpClientFactory;

        public TransactionController(IMediator mediator, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _mediator = mediator;
        }

        // GET: /Transaction
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = await _mediator.Send(new Index.Query());
            return View(model);
        }

        // GET: /Transaction/Details/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _mediator.Send(new Details.Query { Id = id });
            return View(model);
        }

        // GET: /Transaction/CreateComplete
        [HttpGet("CreateComplete")]
        public async Task<IActionResult> CreateComplete()
        {
            var model = await _mediator.Send(new CreateComplete.Query());
            return View(model);
        }

        // POST: /Transaction/CreateComplete
        [HttpPost("CreateComplete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComplete(CreateComplete.Command command)
        {
            if (!ModelState.IsValid)
            {
                var model = await _mediator.Send(new CreateComplete.Query());
                return View(model);
            }

            await _mediator.Send(command);
            TempData["Message"] = "Complete transaction created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Transaction/CreatePartial
        [HttpGet("CreatePartial")]
        public async Task<IActionResult> CreatePartial()
        {
            var model = await _mediator.Send(new CreatePartial.Query());
            return View(model);
        }

        // POST: /Transaction/CreatePartial
        [HttpPost("CreatePartial")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial(CreatePartial.Command command)
        {
            if (!ModelState.IsValid)
            {
                var model = await _mediator.Send(new CreatePartial.Query());
                return View(model);
            }

            await _mediator.Send(command);
            TempData["Message"] = "Partial transaction created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            // Hämta bara den grundläggande informationen som innehåller TransactionType
            var transaction = await _mediator.Send(new Details.Query { Id = id });

            if (transaction == null)
                return NotFound();

            // För Partial
            if (transaction.TransactionType == 0)
            {
                var partialModel = await _mediator.Send(new EditPartial.Query { Id = id });
                return View("EditPartial", partialModel);
            }
            // För Complete
            else if (transaction.TransactionType == 1)
            {
                var completeModel = await _mediator.Send(new EditComplete.Query { Id = id });
                return View("EditComplete", completeModel);
            }

            return NotFound();
        }
        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(int id)
        {
            var model = await _mediator.Send(new EditPartial.Query { Id = id });
            return View("EditPartial", model);
        }

        [HttpPost("EditPartial")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartialConfirmed(EditPartial.Command command)
        {
            if (!ModelState.IsValid)
            {
                var model = await _mediator.Send(new EditPartial.Query { Id = command.Id });
                return View("EditPartial", model);
            }

            await _mediator.Send(command);
            TempData["Message"] = "Partial transaction updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("EditComplete/{id}")]
        public async Task<IActionResult> EditComplete(int id)
        {
            var model = await _mediator.Send(new EditComplete.Query { Id = id });
            return View("EditComplete", model);
        }

        [HttpPost("EditComplete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompleteConfirmed(EditComplete.Command command)
        {
            if (!ModelState.IsValid)
            {
                var model = await _mediator.Send(new EditComplete.Query { Id = command.Id });
                return View("EditComplete", model);
            }

            await _mediator.Send(command);
            TempData["Message"] = "Complete transaction updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Transaction/Delete/{id}
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(Delete.Query query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        }

        // POST: /Transaction/Delete
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Delete.Command command)
        {
            await _mediator.Send(command);
            TempData["Message"] = "Transaction deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet("import-text")]
        public IActionResult ImportText()
        {
            return View("ImportText");
        }

        [HttpPost("import-text")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportTextConfirmed(string transactionData)
        {
            if (string.IsNullOrWhiteSpace(transactionData))
            {
                TempData["Error"] = "Transaction data cannot be empty.";
                return View("ImportText");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("Api");
                var response = await client.PostAsync("transaction/import-text",
                    new StringContent(transactionData, Encoding.UTF8, "text/plain"));

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to import transactions. Error: {errorMessage}";
                    return View("ImportText");
                }

                TempData["Message"] = "Transactions imported successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("ImportText");
            }
        }
        [HttpGet("Categorize/{id}")]
        public async Task<IActionResult> Categorize(int id)
        {
            var model = await _mediator.Send(new Categorize.Query { Id = id });
            return View("Categorize", model);
        }

        [HttpPost("Categorize")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategorizeConfirmed(Categorize.Command command)
        {
            Console.WriteLine($"Transaction ID: {command.Id}");
            Console.WriteLine($"SelectedCategoryId: {command.CategoryId}");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                TempData["Error"] = "Invalid category selection.";
                return RedirectToAction(nameof(Index));
            }

            await _mediator.Send(command);
            TempData["Message"] = "Category saved successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("categorize-uncategorized")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategorizeUncategorized()
        {
            var result = await _mediator.Send(new CategorizeUncategorized.Command());

            TempData["Message"] = $"Processed: {result.TotalProcessed}, Auto-Categorized: {result.AutoCategorized}, Not Categorized: {result.NotCategorized}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("ApproveCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCategory(int id)
        {
            try
            {
                await _mediator.Send(new ApproveCategory.Command { TransactionId = id });
                TempData["Message"] = "Category approved successfully!";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to approve category.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}