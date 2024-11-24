/* using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Piggyzen.Web.Features.Transaction
{
    public class Create
    {
        public class Query : IRequest<Model>
        {
        }

        public class Model
        {
            public CreatePartial.Command PartialCommand { get; set; } = new();
            public CreateComplete.Command CompleteCommand { get; set; } = new();
            public List<SelectListItem> Categories { get; set; } = new();
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

                var model = new Model
                {
                    Categories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList()
                };
                Debug.WriteLine(model.GetType().FullName); // Detta loggar modellens fullständiga typnamn
                                                           // Logga typens fullständiga namn till terminalen
                Console.WriteLine($"Model type Create.cs: {model.GetType().FullName}");

                return model;
            }

            public record Category(int Id, string Name);
        }
        public class Command : IRequest
        {
            public DateTime? BookingDate { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public decimal? Balance { get; set; }
            public int? CategoryId { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public Handler(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");

                // Kontrollera om transaktionen är "Complete" eller "Partial"
                var endpoint = DetermineEndpoint(request);

                var response = await client.PostAsJsonAsync(endpoint, request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to create transaction. Endpoint: {endpoint}");
                }
            }

            private string DetermineEndpoint(Command request)
            {
                // Om BookingDate och Balance är angivna, anta "Complete"
                if (request.BookingDate.HasValue && request.Balance.HasValue)
                {
                    return "transaction/create-complete";
                }

                // Annars anta "Partial"
                return "transaction/create-partial";
            }
        }
    }
} */