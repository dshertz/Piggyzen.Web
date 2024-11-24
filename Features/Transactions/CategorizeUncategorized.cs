namespace Piggyzen.Web.Features.Transaction
{
    using MediatR;

    public class CategorizeUncategorized
    {
        public class Command : IRequest<Result> { }

        public class Result
        {
            public int TotalProcessed { get; set; }
            public int AutoCategorized { get; set; }
            public int NotCategorized { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public CommandHandler(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient("Api");

                // Anropa API
                var response = await client.PostAsJsonAsync("transaction/categorize", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to categorize transactions. Status: {response.StatusCode}");
                }

                var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken);
                if (result == null)
                {
                    throw new InvalidOperationException("Response was null or invalid.");
                }

                return result;
            }
        }
    }
}