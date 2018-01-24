namespace Zebble.Device
{
    using System.Threading.Tasks;
    using Android.Hardware.Fingerprints;
    using Android.OS;
    using Zebble.Device.FingerPrint.Samsung;

    public partial class Fingerprint
    {
        static SamsungFingerPrint SamsungDevice
        {
            get
            {
                var samsungFingerPrint = new SamsungFingerPrint();
                if (samsungFingerPrint.IsCompatible) return samsungFingerPrint;
                return null;
            }
        }

        static FingerprintManager GetService()
        {
            return UIRuntime.GetService<FingerprintManager>(Android.Content.Context.FingerprintService);
        }

        public static Task<bool> IsAvailable(bool allowAlternativeAuthentication = true)
        {
            if (SamsungDevice != null)
                return SamsungDevice.IsAvailable(allowAlternativeAuthentication);
            else
            {
                if (OS.IsAtLeast(BuildVersionCodes.M))
                {
                    var service = GetService();

                    if (service.IsHardwareDetected && service.HasEnrolledFingerprints)
                        return Task.FromResult(result: true);
                }
            }

            return Task.FromResult(result: false);
        }

        static async Task<FingerprintResult> DoAuthenticate(FingerprintRequestConfig config)
        {
            using (var cancellationSignal = new CancellationSignal())
            {
                if (SamsungDevice != null)
                {
                    var failedCallback = new DeafAuthenticationFailedListener();
                    return await SamsungDevice.DoAuthentication(failedCallback, config.CancellationToken);
                }
                else
                {
                    using (config.CancellationToken.Register(cancellationSignal.Cancel))
                    {
                        var callback = new FingerprintAuthenticationCallback();
                        var cryptObjectHelper = new CryptoObjectHelper();
                        GetService().Authenticate(cryptObjectHelper.BuildCryptoObject(), cancellationSignal, FingerprintAuthenticationFlags.None, callback, null);
                        return await callback.Source.Task;
                    }
                }
            }
        }
    }
}
