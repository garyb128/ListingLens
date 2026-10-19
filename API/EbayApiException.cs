namespace ListingLens.API;

public class EbayApiException : Exception
{
    public int StatusCode { get; }

    public EbayApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}