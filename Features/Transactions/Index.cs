using MediatR;

namespace Piggyzen.Web.Features.Transaction
{
    public class Index
    {
        public class Query : IRequest<Model> { }

        public class Model
        {
            public List<TransactionItem> Transactions { get; set; } = new();
        }

        public class TransactionItem
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public DateTime TransactionDate { get; set; }
            public string? CategoryName { get; set; }
            public int TransactionType { get; set; }
            public int CategorizationStatus { get; set; }
            public string? CategorizationStatusDisplay { get; set; }
        }
        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public Handler(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                // Hämta en HttpClient för API-kommunikation
                var client = _httpClientFactory.CreateClient("Api");

                // Hämta transaktioner från API:t
                var response = await client.GetAsync("transaction", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    // Hantera fel
                    throw new HttpRequestException($"Failed to fetch transactions. Status: {response.StatusCode}");
                }

                // Deserialisera API-svaret
                var transactions = await response.Content.ReadFromJsonAsync<List<TransactionItem>>(cancellationToken: cancellationToken);

                if (transactions == null)
                {
                    throw new InvalidOperationException("No transactions returned from API.");
                }

                // Returnera model
                return new Model { Transactions = transactions };
            }
        }
    }
}