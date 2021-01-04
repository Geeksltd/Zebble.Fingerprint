namespace Zebble.Device.Samsung
{
    interface IAuthenticationFailedListener
    {
        void OnFailedTry();
        void OnHelp(FingerprintAuthenticationHelp help, string nativeHelpText);
    }
}