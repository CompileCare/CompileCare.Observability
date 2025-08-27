namespace Sample.Api.Endpoints.Test
{
    public sealed class TestLogging : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("information", async (ILogger<TestLogging> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("This is a test log message from TestLogging endpoint at {Time}", DateTime.UtcNow);
                return Results.Ok(new { message = "Test log message sent. Check your logs.", time = DateTime.UtcNow });
            });

            app.MapGet("/error", (ILogger<Program> logger) =>
            {
                try
                {
                    throw new InvalidOperationException("Something went wrong!");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred in {Endpoint} at {Time}", "/error", DateTime.UtcNow);
                    return Results.Problem("Error logged");
                }
            });

            app.MapGet("/warning", (ILogger<Program> logger) =>
            {
                try
                {
                    throw new InvalidOperationException("Something went wrong!");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "An warnig occurred in {Endpoint} at {Time}", "/waring", DateTime.UtcNow);
                    return Results.Problem("Warning logged");
                }
            });
        }
    }
}
