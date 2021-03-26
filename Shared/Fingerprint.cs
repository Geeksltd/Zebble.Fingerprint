namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Fingerprint;

    public static partial class Fingerprint
    {
        public static Task<bool> IsAvailable(bool allowAlternativeAuthentication = false)
        {
            return CrossFingerprint.Current.IsAvailableAsync(allowAlternativeAuthentication);
        }

        public static async Task<FingerprintResult> Authenticate(FingerprintRequestConfig request, OnError errorAction = OnError.Alert)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                return await Thread.UI.Run(async () =>
                {
                    var config = request.ToConfiguration();
                    var result = await CrossFingerprint.Current.AuthenticateAsync(config, request.CancellationToken);

                    return FingerprintResult.From(result);
                });
            }
            catch (Exception ex)
            {
                await errorAction.Apply(ex, "Failed to access fingerprint: " + ex.Message);
                return new FingerprintResult
                {
                    Status = FingerprintCheckStatus.Failed,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}