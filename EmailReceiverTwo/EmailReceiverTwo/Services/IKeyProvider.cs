using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailReceiverTwo.Services
{
    public interface IKeyProvider
    {
        byte[] EncryptionKey { get; }
        byte[] VerificationKey { get; }
    }
}
