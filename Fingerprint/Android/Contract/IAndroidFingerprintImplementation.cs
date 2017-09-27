using System.Threading;
using System.Threading.Tasks;
using Zebble;

namespace Fingerprint.Contract
{
    public interface IAndroidFingerprintImplementation
    {
        Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken);
    }
}