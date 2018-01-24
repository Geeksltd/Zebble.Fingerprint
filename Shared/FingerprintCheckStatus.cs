namespace Zebble.Device
{
    public enum FingerprintCheckStatus
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
