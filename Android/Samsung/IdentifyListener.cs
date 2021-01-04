namespace Zebble.Device.Samsung
{
    using Com.Samsung.Android.Sdk.Pass;
    using System;
    using System.Threading.Tasks;

    class IdentifyListener : Java.Lang.Object, SpassFingerprint.IIdentifyListener
    {
        readonly Func<SpassFingerprint.IIdentifyListener, Task<bool>> startIdentify;
        readonly IAuthenticationFailedListener failedListener;
        readonly TaskCompletionSource<FingerprintResult> taskCompletionSource;
        int retriesLeft;
        // private TaskCompletionSource<int> _completedSource;

        public IdentifyListener(Func<SpassFingerprint.IIdentifyListener, Task<bool>> startIdentify, IAuthenticationFailedListener failedListener)
        {
            retriesLeft = 2;
            this.startIdentify = startIdentify;
            this.failedListener = failedListener;
            taskCompletionSource = new TaskCompletionSource<FingerprintResult>();
        }

        public async Task<FingerprintResult> GetTask()
        {
            if (!await StartIdentify())
            {
                return new FingerprintResult
                {
                    Status = FingerprintCheckStatus.UnknownError
                };
            }

            return await taskCompletionSource.Task;
        }

        public void CancelManually()
        {
            taskCompletionSource.TrySetResult(new FingerprintResult
            {
                Status = FingerprintCheckStatus.Canceled
            });
        }

        // TODO: use task completion source instead of retries in SamsungFingerprintImplementation, if samsung fixes the library 
        // _completedSource = new TaskCompletionSource<int>();
        async Task<bool> StartIdentify() => await startIdentify(this);

        public void OnCompleted()
        {
            // _completedSource?.TrySetResult(0);
        }

        public async void OnFinished(SpassFingerprintStatus status)
        {
            // _completedSource = new TaskCompletionSource<int>();
            var resultStatus = GetResultStatus(status);

            if (resultStatus == FingerprintCheckStatus.Failed && retriesLeft > 0)
            {
                failedListener?.OnFailedTry();

                if (retriesLeft > 0)
                {
                    retriesLeft--;

                    // await _completedSource.Task;

                    if (await StartIdentify())
                        return;
                }
            }
            else if (resultStatus == FingerprintCheckStatus.Failed && retriesLeft <= 0)
            {
                resultStatus = FingerprintCheckStatus.TooManyAttempts;
            }

            taskCompletionSource.TrySetResult(new FingerprintResult
            {
                Status = resultStatus
            });
        }

        public void OnReady()
        {
        }

        public void OnStarted()
        {
            // TODO: OnStarted -> OnCompleted = failed (to short touched) OMG 
        }

        static FingerprintCheckStatus GetResultStatus(SpassFingerprintStatus status)
        {
            FingerprintCheckStatus resultStatus;
            switch (status)
            {
                case SpassFingerprintStatus.PasswordSuccess:
                case SpassFingerprintStatus.Success:
                    resultStatus = FingerprintCheckStatus.Succeeded;
                    break;
                case SpassFingerprintStatus.QualityFailed:
                case SpassFingerprintStatus.Failed:
                case SpassFingerprintStatus.TimeoutFailed:
                case SpassFingerprintStatus.SensorFailed:
                    resultStatus = FingerprintCheckStatus.Failed;
                    break;
                case SpassFingerprintStatus.UserCancelledByTouchOutside:
                case SpassFingerprintStatus.ButtonPressed:
                case SpassFingerprintStatus.UserCancelled:
                    resultStatus = FingerprintCheckStatus.Canceled;
                    break;
                case SpassFingerprintStatus.OperationDenied:
                    resultStatus = FingerprintCheckStatus.UnknownError;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }

            return resultStatus;
        }
    }
}