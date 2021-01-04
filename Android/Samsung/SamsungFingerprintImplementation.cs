namespace Zebble.Device.Samsung
{
    using Android.App;
    using Android.Util;
    using Plugin.Fingerprint;
    using Java.Lang;
    using System.Threading;
    using System.Threading.Tasks;

    class SamsungFingerPrint
    {
        readonly bool hasNoApi;
        readonly bool hasNoPermission;
        readonly bool hasNoFingerPrintSensor;
        readonly CrossFingerprint spassFingerprint;

        internal bool IsCompatible { get; }

        public SamsungFingerPrint()
        {
            try
            {
                var spass = new Spass();
                spass.Initialize(Application.Context);
                hasNoFingerPrintSensor = !spass.IsFeatureEnabled(Spass.DeviceFingerprint);
                spassFingerprint = new CrossFingerprint() SpassFingerprint(Application.Context);
                IsCompatible = true;
            }
            catch (SecurityException ex)
            {
                Log.Warn(nameof(SamsungFingerPrint), ex);
                hasNoPermission = true;
            }
            catch (Exception ex)
            {
                Log.Warn(nameof(SamsungFingerPrint), ex);
                hasNoApi = true;
            }
        }

        internal async Task<FingerprintResult> DoAuthentication(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken)
        {
            var identifyListener = new IdentifyListener(StartIdentify, failedListener);

            using (cancellationToken.Register(() => TryCancel(identifyListener)))
                return await identifyListener.GetTask();

        }

        internal async Task<bool> IsAvailable(bool allowAlternativeAuthentication = true)
        {
            var resutl = true;

            try
            {
                if (hasNoApi || hasNoPermission || hasNoFingerPrintSensor) resutl = false;
                // On some devices, Samsung doesn't fulfill the API contract of IsFeatureEnabled.
                // This will cause a UnsupportedOperationException when calling HasRegisteredFinger see #53, #70
                else if (!spassFingerprint.HasRegisteredFinger) resutl = false;
            }
            catch (UnsupportedOperationException ex)
            {
                Log.Warn(nameof(SamsungFingerPrint), ex);
                resutl = false;
            }
            catch (Exception ex)
            {
                Log.Warn(nameof(SamsungFingerPrint), ex);
                resutl = false;
            }

            return resutl;
        }

        void TryCancel(IdentifyListener identifyListener)
        {
            try
            {
                spassFingerprint.;
            }
            catch (Exception ex)
            {
                // #75: should be fixed with the reordering of the base.OnPause() in the dialog, but
                // to avoid crashes in other cases, we ignore exceptions here and return cancelled instead
                Log.Warn(nameof(SamsungFingerPrint), ex);
                identifyListener.CancelManually();
            }
        }

        async Task<bool> StartIdentify(SpassFingerprint.IIdentifyListener listener)
        {
            // TODO: remove retry and delay, if samsung fixes the library 
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    spassFingerprint.StartIdentify(listener);
                    return true;
                }
                catch (IllegalStateException ex)
                {
                    Log.Warn(nameof(SamsungFingerPrint), ex);
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    Log.Warn(nameof(SamsungFingerPrint), ex);
                    return false;
                }
            }

            return false;
        }
    }
}