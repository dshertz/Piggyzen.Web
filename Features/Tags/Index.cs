using MediatR;

namespace Piggyzen.Web.Features.Tag
{
    public class Index
    {
        public class Query : IRequest<Model> { }

        public class Model
        {
            public List<TagItem> Tags { get; set; } = new();
        }

        public class TagItem
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

                var response = await client.GetAsync("tag", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to fetch tags. Status: {response.StatusCode}");
                }

                var tags = await response.Content.ReadFromJsonAsync<List<TagItem>>(cancellationToken: cancellationToken);
                if (tags == null)
                {
                    throw new InvalidOperationException("No tags returned from API.");
                }

                return new Model { Tags = tags };
            }
        }
    }
}