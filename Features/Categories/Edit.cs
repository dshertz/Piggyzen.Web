using MediatR;

namespace Piggyzen.Web.Features.Category
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int Id { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? ParentCategoryId { get; set; }
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

                // Hämta kategori från API:t
                var category = await client.GetFromJsonAsync<Command>($"category/{request.Id}", cancellationToken);

                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {request.Id} not found.");
                }

                return category;
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

                // Uppdatera kategori via API:t
                var response = await client.PutAsJsonAsync($"category/{request.Id}", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Failed to update category.");
                }
            }
        }
    }
}