namespace Zebble.Device
{
    using System;
    using Plugin.Fingerprint.Abstractions;

    public class FingerprintResult
    {
        public bool Authenticated => Status == FingerprintCheckStatus.Succeeded;
        public FingerprintCheckStatus Status { get; set; }
        public string ErrorMessage { get; set; }

        internal static FingerprintResult From(FingerprintAuthenticationResult result)
        {
            return new FingerprintResult
            {
                Status = Enum.Parse<FingerprintCheckStatus>(result.Status.ToString()),
                ErrorMessage = result.ErrorMessage
            };
        }
    }
}
