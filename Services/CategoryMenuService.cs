using Microsoft.AspNetCore.Mvc.Rendering;

namespace Piggyzen.Web.Services
{

    public class CategoryMenuService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoryMenuService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<SelectListItem>> GetCategoryMenuAsync(CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("Api");

            // Hämta kategorier från API:et
            var categories = await client.GetFromJsonAsync<List<CategoryMenuItem>>("category", cancellationToken);

            if (categories == null || !categories.Any())
            {
                return new List<SelectListItem>(); // Returnera en tom lista om inga kategorier finns
            }

            // Omvandla till SelectListItem
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name // Anpassa om ParentCategoryName behövs
            }).ToList();
        }

        public class CategoryMenuItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}