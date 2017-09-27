using Android.Hardware.Fingerprints;
using Fingerprint.Contract;
using Java.Lang;
using System.Threading.Tasks;
using Zebble;

namespace Fingerprint.Standard
{
    public class FingerprintAuthenticationCallback : FingerprintManager.AuthenticationCallback, IAuthenticationCallback
    {
        readonly IAuthenticationFailedListener _listener;
        readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;

        public FingerprintAuthenticationCallback(IAuthenticationFailedListener listener)
        {
            _listener = listener;
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
        }

        public Task<FingerprintAuthenticationResult> GetTask() => _taskCompletionSource.Task;


        public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult result)
        {
            base.OnAuthenticationSucceeded(result);
            SetResultSafe(new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Succeeded });
        }

        public override void OnAuthenticationError(FingerprintState errorCode, ICharSequence errString)
        {
            base.OnAuthenticationError(errorCode, errString);
            var message = errString != null ? errString.ToString() : string.Empty;
            var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Failed, ErrorMessage = message };

            if (errorCode == FingerprintState.ErrorLockout)
            {
                result.Status = FingerprintAuthenticationResultStatus.TooManyAttempts;
            }

            SetResultSafe(result);
        }

        void SetResultSafe(FingerprintAuthenticationResult result)
        {
            if (!(_taskCompletionSource.Task.IsCanceled || _taskCompletionSource.Task.IsCompleted || _taskCompletionSource.Task.IsFaulted))
            {
                _taskCompletionSource.SetResult(result);
            }
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
            _listener?.OnFailedTry();
        }

        public override void OnAuthenticationHelp(FingerprintState helpCode, ICharSequence helpString)
        {
            base.OnAuthenticationHelp(helpCode, helpString);
            _listener?.OnHelp(FingerprintAuthenticationHelp.MovedTooFast, helpString?.ToString());
        }
    }
}