namespace Zebble.Device
{
    using System.Threading.Tasks;
    using Foundation;
    using LocalAuthentication;
    using ObjCRuntime;

    public partial class Fingerprint
    {
        static LAContext context;
        static LAContext Context
        {
            get
            {
                if (context == null && OS.IsAtLeastiOS(8)) context = new LAContext();
                return context;
            }
        }

        public static Task<bool> IsAvailable(bool allowAlternativeAuthentication)
        {
            if (Context != null)
            {
                var policy = GetPolicy(allowAlternativeAuthentication);
                if (Context.CanEvaluatePolicy(policy, out var error)) return Task.FromResult(result: true);
            }

            return Task.FromResult(result: false);
        }

        static async Task<FingerprintResult> DoAuthenticate(FingerprintRequestConfig config)
        {
            if (Context.RespondsToSelector(new Selector("localizedFallbackTitle")))
                Context.LocalizedFallbackTitle = config.FallbackTitle;

            if (Context.RespondsToSelector(new Selector("localizedCancelTitle")))
                Context.LocalizedCancelTitle = config.CancelTitle;

            bool successful;
            NSError error;

            using (config.CancellationToken.Register(DisposeContext))
            {
                var policy = GetPolicy(config.AllowAlternativeAuthentication);
                var outcome = await Context.EvaluatePolicyAsync(policy, config.Reason);
                successful = outcome.Item1;
                error = outcome.Item2;
            }

            var result = new FingerprintResult();

            if (successful)
                result.Status = FingerprintCheckStatus.Succeeded;
            else
            {
                result.Status = GetStatus(error);
                result.ErrorMessage = error.LocalizedDescription;
            }

            DisposeContext();
            return result;
        }

        static FingerprintCheckStatus GetStatus(NSError error)
        {
            switch ((LAStatus)(int)error.Code)
            {
                case LAStatus.AuthenticationFailed:
                    var description = error.Description;
                    if (description != null && description.Contains("retry limit exceeded"))
                        return FingerprintCheckStatus.TooManyAttempts;
                    else
                        return FingerprintCheckStatus.Failed;

                case LAStatus.UserCancel:
                case LAStatus.AppCancel:
                    return FingerprintCheckStatus.Canceled;

                case LAStatus.UserFallback: return FingerprintCheckStatus.FallbackRequested;

                case LAStatus.TouchIDLockout: return FingerprintCheckStatus.TooManyAttempts;

                default: return FingerprintCheckStatus.UnknownError;
            }
        }

        static LAPolicy GetPolicy(bool allowAlternativeAuthentication)
        {
            return allowAlternativeAuthentication ?
                LAPolicy.DeviceOwnerAuthentication :
                LAPolicy.DeviceOwnerAuthenticationWithBiometrics;
        }

        static void DisposeContext()
        {
            using (context)
            {
                if (context?.RespondsToSelector(new Selector("invalidate")) == true)
                    context?.Invalidate();
            }

            context = null;
        }
    }
}