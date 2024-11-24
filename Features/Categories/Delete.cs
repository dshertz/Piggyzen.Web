using MediatR;

namespace Piggyzen.Web.Features.Category
{
    public class Delete
    {
        public class Query : IRequest<Command>
        {
            public int Id { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
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
                var response = await client.DeleteAsync($"category/{request.Id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Failed to delete category.");
                }
            }
        }
    }
}