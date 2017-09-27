namespace Zebble
{
    using global::Fingerprint.Samsung;
    using global::Fingerprint.Standard;
    using System;
    using System.Threading;

    public partial class Fingerprint
    {
        private static Lazy<IFingerprint> _implementation = new Lazy<IFingerprint>(CreateFP, LazyThreadSafetyMode.PublicationOnly);
        public static IFingerprint Current => _implementation.Value;

        private static IFingerprint CreateFP()
        {
            var samsungFp = new SamsungFingerprintImplementation();

            if (samsungFp.IsCompatible)
                return samsungFp;

            return new StandardFingerprintImplementation();
        }

        public static void Dispose()
        {
            if (_implementation != null && _implementation.IsValueCreated)
            {
                _implementation = new Lazy<IFingerprint>(CreateFP, LazyThreadSafetyMode.PublicationOnly);
            }
        }
    }
}