using MediatR;

namespace Piggyzen.Web.Features.Transaction
{
    public class ImportFromText
    {
        public class Command : IRequest<Result>
        {
            public string TransactionData { get; set; }
        }

        public class Result
        {
            public string Message { get; set; }
            public int Count { get; set; }
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

                // Skicka POST-anrop till API:et
                var response = await client.PostAsJsonAsync("transaction/import-text", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("Failed to import transactions from text.");
                }

                var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: cancellationToken);

                if (result == null)
                {
                    throw new InvalidOperationException("No result returned from the API.");
                }

                return result;
            }
        }
    }
}