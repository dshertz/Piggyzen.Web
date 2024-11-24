using MediatR;

namespace Piggyzen.Web.Features.Tag
{
    public class Create
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public int Type { get; set; } // Will be mapped to TagTypeEnum
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

                var response = await client.PostAsJsonAsync("tag", request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Failed to create tag.");
                }
            }
        }
    }
}