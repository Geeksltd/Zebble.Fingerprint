namespace Zebble.Device
{
    using Plugin.Fingerprint;

    partial class Fingerprint
    {
        static Fingerprint()
        {
            Thread.UI.Run(() => CrossFingerprint.SetCurrentActivityResolver(() => UIRuntime.CurrentActivity));
        }
    }
}