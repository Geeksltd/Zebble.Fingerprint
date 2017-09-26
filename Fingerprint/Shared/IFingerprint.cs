using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zebble
{
    public interface IFingerprint
    {
        /// <summary>
        /// Checks the availability of fingerprint authentication.
        /// Checks are performed in this order:
        /// 1. API supports accessing the fingerprint sensor
        /// 2. Permission for accessint the fingerprint sensor granted
        /// 3. Device has sensor
        /// 4. Fingerprint has been enrolled
        /// <see cref="FingerprintAvailability.Unknown"/> will be returned if the check failed 
        /// with some other platform specific reason.
        /// </summary>
        /// <param name="allowAlternativeAuthentication">
        /// En-/Disables the use of the PIN / Passwort as fallback.
        /// Supported Platforms: iOS, Mac
        /// Default: false
        /// </param>
        Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false);

        /// <summary>
        /// Checks if <see cref="GetAvailabilityAsync"/> returns <see cref="FingerprintAvailability.Available"/>.
        /// </summary>
        /// <param name="allowAlternativeAuthentication">
        /// En-/Disables the use of the PIN / Passwort as fallback.
        /// Supported Platforms: iOS, Mac
        /// Default: false
        /// </param>
        /// <returns><c>true</c> if Available, else <c>false</c></returns>
        Task<bool> IsAvailableAsync(bool allowAlternativeAuthentication = false);

        /// <summary>
        /// Requests the authentication.
        /// </summary>
        /// <param name="reason">Reason for the fingerprint authentication request. Displayed to the user.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>Authentication result</returns>
        Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Requests the authentication.
        /// </summary>
        /// <param name="authRequestConfig">Configuration of the dialog that is displayed to the user.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>Authentication result</returns>
        Task<FingerprintAuthenticationResult> AuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Indicates if a fingerprint authentication can be performed.
    /// </summary>
    public enum FingerprintAvailability
    {
        /// <summary>
        /// Fingerprint authentication can be used.
        /// </summary>
        Available,
        /// <summary>
        /// This plugin has no implementation for the current platform.
        /// </summary>
        NoImplementation,
        /// <summary>
        /// Operating system has no supported fingerprint API.
        /// </summary>
        NoApi,
        /// <summary>
        /// App is not allowed to access the fingerprint sensor.
        /// </summary>
        NoPermission,
        /// <summary>
        /// Device has no fingerprint sensor.
        /// </summary>
        NoSensor,
        /// <summary>
        /// Fingerprint has not been set up.
        /// </summary>
        NoFingerprint,
        /// <summary>
        /// An unknown, platform specific error occurred. Availability status could not be 
        /// mapped to a <see cref="FingerprintAvailability"/>.
        /// </summary>
        Unknown
    }

    public class FingerprintAuthenticationResult
    {
        /// <summary>
        /// Indicatates whether the authentication was successful or not.
        /// </summary>
        public bool Authenticated { get { return Status == FingerprintAuthenticationResultStatus.Succeeded; } }

        /// <summary>
        /// Detailed information of the authentication.
        /// </summary>
        public FingerprintAuthenticationResultStatus Status { get; set; }

        /// <summary>
        /// Reason for the unsucessful authentication.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Configuration of the stuff presented to the user.
    /// </summary>
    public class AuthenticationRequestConfiguration
    {
        /// <summary>
        /// Reason of the authentication request.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Title of the cancel button.
        /// </summary>
        public string CancelTitle { get; set; }

        /// <summary>
        /// Title of the fallback button.
        /// </summary>
        public string FallbackTitle { get; set; }

        /// <summary>
        /// En-/Disables the dialog. 
        /// Supported Platforms: Android
        /// Default: true
        /// </summary>
        public bool UseDialog { get; set; } = true;

        /// <summary>
        /// Shown when a recoverable error has been encountered during authentication. 
        /// The help strings are provided to give the user guidance for what went wrong.
        /// If a string is null or empty, the string provided by Android is shown.
        /// Supported Platforms: Android (when UseDialog is true)
        /// </summary>
        public AuthenticationHelpTexts HelpTexts { get; }

        /// <summary>
        /// En-/Disables the use of the PIN / Passwort as fallback.
        /// Supported Platforms: iOS, Mac
        /// Default: false
        /// </summary>
        public bool AllowAlternativeAuthentication { get; set; } = false;

        public AuthenticationRequestConfiguration(string reason)
        {
            Reason = reason;
            HelpTexts = new AuthenticationHelpTexts();
        }
    }

    public class AuthenticationHelpTexts
    {
        /// <summary>
        /// The fingerprint image was incomplete due to quick motion.
        /// </summary>
        public string MovedTooFast { get; set; }

        /// <summary>
        /// The fingerprint image was unreadable due to lack of motion.
        /// </summary>
        public string MovedTooSlow { get; set; }

        /// <summary>
        /// Only a partial fingerprint image was detected.
        /// </summary>
        public string Partial { get; set; }

        /// <summary>
        /// The fingerprint image was too noisy to process due to a detected condition.
        /// </summary>
        public string Insufficient { get; set; }

        /// <summary>
        /// The fingerprint image was too noisy due to suspected or detected dirt on the sensor.
        /// </summary>
        public string Dirty { get; set; }
    }

    public enum FingerprintAuthenticationResultStatus
    {
        Unknown,
        Succeeded,
        FallbackRequested,
        Failed,
        Canceled,
        TooManyAttempts,
        UnknownError,
        NotAvailable
    }


}
