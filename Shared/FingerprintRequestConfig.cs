namespace Zebble.Device
{
    using System.Threading;

    public class FingerprintRequestConfig
    {
        public string Reason { get; internal set; }
        public string CancelTitle { get; set; }
        public string FallbackTitle { get; set; }
        public bool UseDialog { get; set; } = true;
        public bool AllowAlternativeAuthentication { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
