namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;
    using Windows.Security.Credentials.UI;

    public partial class Fingerprint
    {
        static async Task<FingerprintResult> DoAuthenticate(FingerprintRequestConfig authRequestConfig)
        {
            var result = new FingerprintResult();
            try
            {
                switch (await UserConsentVerifier.RequestVerificationAsync(authRequestConfig.Reason))
                {
                    case UserConsentVerificationResult.Verified:
                        result.Status = FingerprintCheckStatus.Succeeded;
                        break;

                    case UserConsentVerificationResult.DeviceBusy:
                    case UserConsentVerificationResult.DeviceNotPresent:
                    case UserConsentVerificationResult.DisabledByPolicy:
                    case UserConsentVerificationResult.NotConfiguredForUser:
                        result.Status = FingerprintCheckStatus.NotAvailable;
                        break;

                    case UserConsentVerificationResult.RetriesExhausted:
                        result.Status = FingerprintCheckStatus.TooManyAttempts;
                        break;
                    case UserConsentVerificationResult.Canceled:
                        result.Status = FingerprintCheckStatus.Canceled;
                        break;
                    default:
                        result.Status = FingerprintCheckStatus.Failed;
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Status = FingerprintCheckStatus.UnknownError;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public static async Task<bool> IsAvailable(bool allowAlternativeAuthentication)
        {
            return await UserConsentVerifier.CheckAvailabilityAsync() == UserConsentVerifierAvailability.Available;
        }
    }
}
