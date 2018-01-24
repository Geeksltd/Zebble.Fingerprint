namespace Zebble.Device
{
    using Android.Security.Keystore;
    using Java.Security;
    using Javax.Crypto;
    using static Android.Hardware.Fingerprints.FingerprintManager;

    public class CryptoObjectHelper
    {
        // ReSharper disable InconsistentNaming
        static readonly string TAG = "X:" + typeof(CryptoObjectHelper).Name;

        // This can be key name you want. Should be unique for the app.
        static readonly string KEY_NAME = "BasicFingerPrintSample.FingerprintManagerAPISample.sample_fingerprint_key";

        // We always use this keystore on Android.
        static readonly string KEYSTORE_NAME = "AndroidKeyStore";

        // Should be no need to change these values.
        static readonly string KEY_ALGORITHM = KeyProperties.KeyAlgorithmAes;
        static readonly string BLOCK_MODE = KeyProperties.BlockModeCbc;
        static readonly string ENCRYPTION_PADDING = KeyProperties.EncryptionPaddingPkcs7;

        static readonly string TRANSFORMATION = KEY_ALGORITHM + "/" +
                                                BLOCK_MODE + "/" +
                                                ENCRYPTION_PADDING;
        // ReSharper restore InconsistentNaming

        readonly KeyStore KeyStore;

        public CryptoObjectHelper()
        {
            KeyStore = KeyStore.GetInstance(KEYSTORE_NAME);
            KeyStore.Load(null);
        }

        public CryptoObject BuildCryptoObject()
        {
            var cipher = CreateCipher();
            return new CryptoObject(cipher);
        }

        Cipher CreateCipher(bool retry = true)
        {
            var key = GetKey();
            var cipher = Cipher.GetInstance(TRANSFORMATION);
            try
            {
                cipher.Init(CipherMode.EncryptMode, key);
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                // Log.Debug(TAG, "The key was invalidated, creating a new key.");
                KeyStore.DeleteEntry(KEY_NAME);
                if (retry)
                {
                    CreateCipher(retry: false);
                }
                else
                {
                    //    throw new Exception("Could not create the cipher for fingerprint authentication.", e);
                }
            }

            return cipher;
        }

        IKey GetKey()
        {
            if (!KeyStore.IsKeyEntry(KEY_NAME))
            {
                CreateKey();
            }

            return KeyStore.GetKey(KEY_NAME, null);
        }

        void CreateKey()
        {
            var keyGen = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KEYSTORE_NAME);
            var keyGenSpec =
                new KeyGenParameterSpec.Builder(KEY_NAME, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(BLOCK_MODE)
                    .SetEncryptionPaddings(ENCRYPTION_PADDING)
                    .SetUserAuthenticationRequired(required: true)
                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
            Device.Log.Message("New key created for fingerprint authentication.");
        }
    }
}