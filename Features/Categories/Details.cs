using MediatR;

namespace Piggyzen.Web.Features.Category
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
            public string ParentCategoryName { get; set; }
            public List<Subcategory> Subcategories { get; set; } = new();

            public class Subcategory
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
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

                var category = await client.GetFromJsonAsync<Model>($"category/{request.Id}", cancellationToken);

                if (category == null)
                {
                    throw new KeyNotFoundException("Category not found.");
                }

                return category;
            }
        }
    }
}