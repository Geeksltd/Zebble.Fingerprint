namespace Zebble.Device.FingerPrint.Samsung
{
    public interface IAuthenticationFailedListener
    {
        void OnFailedTry();
        void OnHelp(FingerprintAuthenticationHelp help, string nativeHelpText);
    }
}