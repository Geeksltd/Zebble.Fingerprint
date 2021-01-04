namespace Zebble.Device
{
    using Android.Hardware.Fingerprints;
    using Java.Lang;
    using System;
    using System.Threading.Tasks;
    using static Android.Hardware.Fingerprints.FingerprintManager;
    using Olive;

    class FingerprintAuthenticationCallback : AuthenticationCallback
    {
        public TaskCompletionSource<FingerprintResult> Source = new TaskCompletionSource<FingerprintResult>();
        static readonly byte[] SECRET_BYTES = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public override void OnAuthenticationSucceeded(AuthenticationResult nativeResult)
        {
            base.OnAuthenticationSucceeded(nativeResult);

            var result = new FingerprintResult { Status = FingerprintCheckStatus.Succeeded };

            if (nativeResult.CryptoObject.Cipher != null)
            {
                try
                {
                    var message = nativeResult.CryptoObject.Cipher.DoFinal(SECRET_BYTES);
                    result.Result = Convert.ToBase64String(message);
                }
                catch (System.Exception ex)
                {
                    Device.Log.Warning("Failed to decode the success message: " + ex.Message);
                }
            }

            Source.TrySetResult(result);
        }

        public override void OnAuthenticationError(FingerprintState errorCode, ICharSequence errString)
        {
            base.OnAuthenticationError(errorCode, errString);

            var result = new FingerprintResult
            {
                Status = FingerprintCheckStatus.Failed,
                ErrorMessage = errString.ToStringOrEmpty()
            };

            if (errorCode == FingerprintState.ErrorLockout)
                result.Status = FingerprintCheckStatus.TooManyAttempts;

            Source.TrySetResult(result);
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();

            var result = new FingerprintResult
            {
                Status = FingerprintCheckStatus.Failed,
                ErrorMessage = "Authentication Failed"
            };

            Source.TrySetResult(result);
        }
    }
}