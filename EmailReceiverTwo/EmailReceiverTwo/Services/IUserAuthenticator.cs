using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmailReceiverTwo.Services
{
    public interface IUserAuthenticator
    {
        bool TryAuthenticateUser(string username, string password, out IList<Claim> claims);
    }
}
