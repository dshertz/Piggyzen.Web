using MediatR;

namespace Piggyzen.Web.Features.Transaction
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
                var transaction = await client.GetFromJsonAsync<Command>($"transaction/{request.Id}", cancellationToken);

                if (transaction == null)
                {
                    throw new KeyNotFoundException($"Transaction with ID {request.Id} not found.");
                }

                return transaction;
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
                var response = await client.DeleteAsync($"transaction/{request.Id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Failed to delete transaction.");
                }
            }
        }
    }
}