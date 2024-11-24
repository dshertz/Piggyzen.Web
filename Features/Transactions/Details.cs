using MediatR;

namespace Piggyzen.Web.Features.Transaction
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
            public DateTime? BookingDate { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public decimal? Balance { get; set; }
            public string? CategoryName { get; set; }
            public string? ParentCategoryName { get; set; }
            public string? Memo { get; set; }
            public int VerificationStatus { get; set; }
            public int CategorizationStatus { get; set; }
            public int TransactionType { get; set; }
            public string? VerificationStatusDisplay { get; set; }
            public string? CategorizationStatusDisplay { get; set; }
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
                var response = await client.GetAsync($"transaction/{request.Id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to fetch transaction details. Status: {response.StatusCode}");
                }

                var transaction = await response.Content.ReadFromJsonAsync<Model>(cancellationToken: cancellationToken);

                if (transaction == null)
                {
                    throw new InvalidOperationException("No transaction details returned from API.");
                }

                return transaction;
            }
        }
    }
}