using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailReceiverTwo.Services
{
    public interface ICryptoService
    {
        byte[] Protect(byte[] plainText);
        byte[] Unprotect(byte[] payload);
        string CreateSalt();
        string CreateToken(string value);
        string GetValueFromToken(string token);
    }
}
