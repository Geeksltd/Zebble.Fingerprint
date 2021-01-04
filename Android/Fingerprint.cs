namespace Zebble.Device
{
    using Plugin.Fingerprint;

    partial class Fingerprint
    {
        static Fingerprint()
        {
            CrossFingerprint.SetCurrentActivityResolver(() => UIRuntime.CurrentActivity);
        }
    }
}