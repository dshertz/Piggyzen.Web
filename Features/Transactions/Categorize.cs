namespace Piggyzen.Web.Features.Transaction
{
    using System.ComponentModel.DataAnnotations;
    using MediatR;

    public class Categorize
    {
        public class Query : IRequest<Command>
        {
            public int Id { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; } // Representerar transaktions-ID
            public int? CategoryId { get; set; } // Kategorival
            public List<CategoryModel> Categories { get; set; } = new(); // Tillgängliga kategorier
        }

        public class CategoryModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<CategoryModel> Subcategories { get; set; } = new();
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public QueryHandler(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");

                // Hämta transaktion
                var response = await client.GetAsync($"transaction/{request.Id}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new KeyNotFoundException("Transaction not found.");
                }

                var transactionData = await response.Content.ReadFromJsonAsync<Command>(cancellationToken);
                if (transactionData == null)
                {
                    throw new KeyNotFoundException("Transaction data is null.");
                }

                // Hämta kategorihierarkin
                var categoryResponse = await client.GetAsync("category/hierarchy", cancellationToken);
                if (!categoryResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to load categories.");
                }

                // Lägg till kategorier
                var categories = await categoryResponse.Content.ReadFromJsonAsync<List<CategoryModel>>(cancellationToken);
                transactionData.Categories = categories ?? new List<CategoryModel>();

                return transactionData;
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
                if (!request.CategoryId.HasValue)
                {
                    throw new ValidationException("CategoryId is required.");
                }

                var client = _httpClientFactory.CreateClient("Api");

                // Skapa payload och anropa API
                var response = await client.PutAsync(
                    $"transaction/{request.Id}/category/{request.CategoryId.Value}",
                    null, // Ingen body behövs
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to update category. Status: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }
            }
        }
    }
}