using MediatR;

namespace Piggyzen.Web.Features.Category
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public List<Category> Categories { get; set; }

            public class Category
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string? ParentCategoryName { get; set; }
            }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public Handler(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");

                var categories = await client.GetFromJsonAsync<List<Result.Category>>("category", cancellationToken);

                if (categories == null || !categories.Any())
                {
                    throw new InvalidOperationException("No categories returned from API.");
                }

                return new Result { Categories = categories };
            }
        }
    }
}