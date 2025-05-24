using Microsoft.AspNetCore.SignalR;

namespace Riffle.Hubs;

public class LoggingHubFilter : IHubFilter
{
    private readonly ILogger<LoggingHubFilter> _logger;

    public LoggingHubFilter(ILogger<LoggingHubFilter> logger)
    {
        _logger = logger;
    }

#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
    public async ValueTask<object> InvokeMethodAsync(
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        HubInvocationContext invocationContext, 
        Func<HubInvocationContext, ValueTask<object>> next)
    {
        try
        {
            _logger.LogInformation("Ran hub method: {HubMethod}", invocationContext.HubMethodName);
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Hub method {HubMethod} for connection {ConnectionId}",
                invocationContext.HubMethodName,
                invocationContext.Context.ConnectionId);
            throw;
        }
    }
}
