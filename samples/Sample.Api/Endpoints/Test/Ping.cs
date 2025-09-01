
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sample.Api.Endpoints.Test
{
    public sealed class Ping : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("ping", async (CancellationToken cancellationToken) =>
            {
                return Results.Ok(new { message = "hello Shatrughna !!", time = DateTime.UtcNow });
            });
        }
    }
}
