using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Piggyzen.Web.Services;

namespace Piggyzen.Web.Features.Category
{
    public class Create
    {
        public class Query : IRequest<Model> { }

        public class Model
        {
            public string Name { get; set; }
            public int? ParentCategoryId { get; set; }
            public bool IsActive { get; set; }
            public List<SelectListItem> Categories { get; set; } = new();
        }
        public class Command : IRequest
        {
            public string Name { get; set; }
            public int? ParentCategoryId { get; set; }
            public bool IsActive { get; set; } = true;
        }
        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly CategoryMenuService _categoryMenuService;

            public QueryHandler(CategoryMenuService categoryMenuService)
            {
                _categoryMenuService = categoryMenuService;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                var categories = await _categoryMenuService.GetCategoryMenuAsync(cancellationToken);

                return new Model
                {
                    Categories = categories
                };
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

                var response = await client.PostAsJsonAsync("category", request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Failed to create category.");
                }
            }
        }
    }
}