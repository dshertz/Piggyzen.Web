using MediatR;

namespace Piggyzen.Web.Features.Tag
{
    public class Details
    {
        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
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
                var client = _httpClientFactory.CreateClient("Api");

                var tag = await client.GetFromJsonAsync<Model>($"tag/{request.Id}", cancellationToken);

                if (tag == null)
                {
                    throw new KeyNotFoundException("Tag not found.");
                }

                return tag;
            }
        }
    }
}