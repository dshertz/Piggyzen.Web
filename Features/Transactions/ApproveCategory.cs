namespace Piggyzen.Web.Features.Transaction
{
    using MediatR;

    public class ApproveCategory
    {
        public class Command : IRequest
        {
            public int TransactionId { get; set; }
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

                // Skicka en POST-begäran till API:et
                var response = await client.PostAsync(
                    $"transaction/ApproveCategory/{request.TransactionId}",
                    null,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to approve category for transaction ID {request.TransactionId}.");
                }
            }
        }
    }
}