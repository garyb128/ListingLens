using System.Net;

namespace ListingLens.API;

internal static class HttpRetry
{
    const int MaxAttempts = 3;

    static readonly TimeSpan BaseDelay = TimeSpan.FromMilliseconds(500);
    static readonly TimeSpan MaxRetryAfter = TimeSpan.FromSeconds(5);

    public static async Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        Func<HttpRequestMessage> requestFactory,
        CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            // HttpRequestMessage cannot be sent twice, so create a fresh one each time
            using var request = requestFactory();

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException) when (attempt < MaxAttempts)
            {
                await DelayAsync(attempt, cancellationToken);
                continue;
            }
            catch (OperationCanceledException) when (attempt < MaxAttempts && !cancellationToken.IsCancellationRequested)
            {
                await DelayAsync(attempt, cancellationToken);
                continue;
            }

            if (!IsRetryable(response.StatusCode) || attempt == MaxAttempts)
            {
                return response;
            }

            var delay = GetDelay(response, attempt);
            response.Dispose();
            System.Diagnostics.Debug.WriteLine(
                $"Retrying eBay request in {delay.TotalMilliseconds:N0} ms " +
                $"(attempt {attempt + 1}/{MaxAttempts}).");

            await Task.Delay(delay, cancellationToken);
        }
    }

    static bool IsRetryable(HttpStatusCode statusCode)
    {
        return statusCode is
            HttpStatusCode.RequestTimeout or
            HttpStatusCode.TooManyRequests or
            HttpStatusCode.InternalServerError or
            HttpStatusCode.BadGateway or
            HttpStatusCode.ServiceUnavailable or
            HttpStatusCode.GatewayTimeout;
    }

    static TimeSpan GetDelay(HttpResponseMessage response, int attempt)
    {
        var retryAfter = response.Headers.RetryAfter;

        if (retryAfter?.Delta is { } delta && delta > TimeSpan.Zero)
        {
            return delta < MaxRetryAfter ? delta : MaxRetryAfter;
        }

        if (retryAfter?.Date is { } date)
        {
            var wait = date - DateTimeOffset.UtcNow;

            if (wait > TimeSpan.Zero)
            {
                return wait < MaxRetryAfter ? wait : MaxRetryAfter;
            }
        }

        return Backoff(attempt);
    }

    static async Task DelayAsync(
        int attempt,
        CancellationToken cancellationToken)
    {
        var delay = Backoff(attempt);

        System.Diagnostics.Debug.WriteLine(
            $"Retrying eBay request in {delay.TotalMilliseconds:N0} ms " +
            $"(attempt {attempt + 1}/{MaxAttempts}).");

        await Task.Delay(delay, cancellationToken);
    }

    static TimeSpan Backoff(int attempt)
    {
        var baseDelay = TimeSpan.FromMilliseconds(
            BaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));

        var jitter = TimeSpan.FromMilliseconds(
            Random.Shared.Next(0, 250));

        return baseDelay + jitter;
    }
}