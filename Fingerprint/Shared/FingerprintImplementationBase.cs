﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zebble
{
    public abstract class FingerprintImplementationBase : IFingerprint
    {
        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken = default(CancellationToken))
        {
            return AuthenticateAsync(new AuthenticationRequestConfiguration(reason), cancellationToken);
        }

        public async Task<FingerprintAuthenticationResult> AuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!await IsAvailableAsync(authRequestConfig.AllowAlternativeAuthentication))
                return new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };

            return await NativeAuthenticateAsync(authRequestConfig, cancellationToken);
        }

        public async Task<bool> IsAvailableAsync(bool allowAlternativeAuthentication = false)
        {
            return await GetAvailabilityAsync(allowAlternativeAuthentication) == FingerprintAvailability.Available;
        }

        public abstract Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false);
        protected abstract Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken);
    }
}
