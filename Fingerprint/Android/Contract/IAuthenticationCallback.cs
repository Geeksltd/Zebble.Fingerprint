using System;
using System.Threading.Tasks;
using Zebble;

namespace Fingerprint.Contract
{
    public interface IAuthenticationCallback : IDisposable
    {
        Task<FingerprintAuthenticationResult> GetTask();
    }
}