using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Piggyzen.Web.Services;

namespace Piggyzen.Web.Features.Transaction
{
    public class EditPartial
    {
        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public DateTime TransactionDate { get; set; } // Lägg till TransactionDate
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public string? Memo { get; set; }
            public int? CategoryId { get; set; }
            public List<SelectListItem> Categories { get; set; } = new();

        }

        public class Command : IRequest
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public DateTime TransactionDate { get; set; } // Lägg till TransactionDate
            public string? Memo { get; set; } // Lägg till Memo
            public int? CategoryId { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly CategoryMenuService _categoryMenuService;

            public QueryHandler(IHttpClientFactory httpClientFactory, CategoryMenuService categoryMenuService)
            {
                _categoryMenuService = categoryMenuService;
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");
                var response = await client.GetFromJsonAsync<Model>($"transaction/{request.Id}", cancellationToken);

                if (response == null)
                {
                    throw new InvalidOperationException($"Transaction with ID {request.Id} not found.");
                }
                // Lägg till kategorier från tjänsten
                response.Categories = await _categoryMenuService.GetCategoryMenuAsync(cancellationToken);


                return response;
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public CommandHandler(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");
                var response = await client.PutAsJsonAsync($"transaction/partial-update/{request.Id}", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("Failed to update partial transaction.");
                }
            }
        }
    }
}