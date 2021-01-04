namespace Zebble.Device
{
    using System;
    using System.Threading;
    using Olive;
    using Plugin.Fingerprint.Abstractions;

    public class FingerprintRequestConfig
    {
        public string Title { get; }
        public string Reason { get; }
        public string CancelTitle { get; set; }
        public string FallbackTitle { get; set; }
        public FingerprintRequestHelpTexts HelpTexts { get; }
        public bool AllowAlternativeAuthentication { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public FingerprintRequestConfig(string title, string reason)
        {
            if (Title.IsEmpty())
                throw new ArgumentNullException(nameof(Title));

            if (Reason.IsEmpty())
                throw new ArgumentNullException(nameof(Reason));

            Reason = reason;
            Title = title;
            HelpTexts = new FingerprintRequestHelpTexts();
        }

        internal AuthenticationRequestConfiguration ToConfiguration()
        {
            return new(Title, Reason)
            {
                CancelTitle = CancelTitle,
                FallbackTitle = FallbackTitle,
                HelpTexts =
                {
                    MovedTooFast = HelpTexts.MovedTooFast,
                    MovedTooSlow = HelpTexts.MovedTooSlow,
                    Partial = HelpTexts.Partial,
                    Insufficient = HelpTexts.Insufficient,
                    Dirty = HelpTexts.Dirty,
                },
                AllowAlternativeAuthentication = AllowAlternativeAuthentication
            };
        }
    }

    public class FingerprintRequestHelpTexts
    {
        public string MovedTooFast { get; set; }
        public string MovedTooSlow { get; set; }
        public string Partial { get; set; }
        public string Insufficient { get; set; }
        public string Dirty { get; set; }
    }
}
