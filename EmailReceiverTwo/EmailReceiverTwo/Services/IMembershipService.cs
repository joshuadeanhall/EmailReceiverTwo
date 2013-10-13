using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EmailReceiverTwo.Domain;

namespace EmailReceiverTwo.Services
{
    public interface IMembershipService
    {
        // Account creation
        EmailUser AddUser(ClaimsPrincipal claimsPrincipal);
        void LinkIdentity(EmailUser user, ClaimsPrincipal principal);
        EmailUser AddUser(EmailUser user);

        void ChangeUserName(EmailUser user, string newUserName);

        // Password management
        void ChangeUserPassword(EmailUser user, string oldPassword, string newPassword);
        void SetUserPassword(EmailUser user, string password);
        void RequestResetPassword(EmailUser user, int requestResetPasswordValidThroughInHours);
        void ResetUserPassword(EmailUser user, string newPassword);

        string GetUserNameFromToken(string token);

        bool TryAuthenticateUser(string userName, string password, out EmailUser user);
    }
}
