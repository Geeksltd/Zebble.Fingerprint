namespace Zebble
{
    using System;
    using System.Threading.Tasks;

    namespace Device
    {
        public static partial class Fingerprint
        {
            public static async Task<FingerprintResult> Authenticate(string reason,
                FingerprintRequestConfig requestConfig = null,
                OnError errorAction = OnError.Alert)
            {
                requestConfig = requestConfig ?? new FingerprintRequestConfig();
                requestConfig.Reason = reason;

                try
                {
                    if (!await IsAvailable(requestConfig.AllowAlternativeAuthentication))
                        return new FingerprintResult { Status = FingerprintCheckStatus.NotAvailable };

                    return await DoAuthenticate(requestConfig);
                }
                catch (Exception ex)
                {
                    await errorAction.Apply(ex, "Failed to access fingerprint : " + ex.Message);
                    return null;
                }
            }
        }
    }
}