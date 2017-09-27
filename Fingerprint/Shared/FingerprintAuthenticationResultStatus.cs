namespace Zebble
{
    public enum FingerprintAuthenticationResultStatus
    {
        Unknown,
        Succeeded,
        FallbackRequested,
        Failed,
        Canceled,
        TooManyAttempts,
        UnknownError,
        NotAvailable
    }
}
