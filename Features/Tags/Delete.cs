using MediatR;

namespace Piggyzen.Web.Features.Tag
{
    public class Delete
    {
        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Type { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public QueryHandler(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");

                var tag = await client.GetFromJsonAsync<Model>($"tag/{request.Id}", cancellationToken);
                if (tag == null)
                {
                    throw new KeyNotFoundException($"Tag with ID {request.Id} not found.");
                }

                return tag;
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public CommandHandler(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");

                var response = await client.DeleteAsync($"tag/{request.Id}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Failed to delete tag.");
                }
            }
        }
    }
}