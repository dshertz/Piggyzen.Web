using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Piggyzen.Web.Features.Transaction
{
    public class CreatePartial
    {
        public class Query : IRequest<Model>
        {
        }

        public class Model
        {
            public Command Command { get; set; } = new();
            public List<SelectListItem> Categories { get; set; } = new();
        }

        public class Command : IRequest
        {
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public int? CategoryId { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public QueryHandler(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");
                var categories = await client.GetFromJsonAsync<List<Category>>("category", cancellationToken);

                return new Model
                {
                    Categories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList()
                };
            }

            public record Category(int Id, string Name);
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
                var response = await client.PostAsJsonAsync("transaction/create-partial", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Failed to create partial transaction.");
                }
            }
        }
    }
}