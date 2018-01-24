namespace Zebble.Device
{
    public class FingerprintResult
    {
        public bool Authenticated => Status == FingerprintCheckStatus.Succeeded;
        public FingerprintCheckStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public string Result { get; set; } = string.Empty;
    }
}
