using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using EmailReceiverTwo.Domain;
using EmailReceiverTwo.Infrastructure;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace EmailReceiverTwo.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IDocumentSession _session;
        private readonly ICryptoService _crypto;

        private const int passwordMinLength = 6;

        public MembershipService(IDocumentSession session, ICryptoService crypto)
        {
            _session = session;
            _crypto = crypto;
        }

        public EmailUser AddUser(ClaimsPrincipal claimsPrincipal)
        {
            var identity = claimsPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);
            var name = claimsPrincipal.GetClaimValue(ClaimTypes.Name);
            var email = claimsPrincipal.GetClaimValue(ClaimTypes.Email);
            var providerName = claimsPrincipal.GetIdentityProvider();

            return AddUser(name, providerName, identity, email);
        }

        private EmailUser AddUser(string userName, string providerName, string identity, string email)
        {
            if (!IsValidUserName(userName))
            {
                throw new InvalidOperationException(String.Format("{0} is not a valid user name.", userName));
            }

            EnsureProviderAndIdentityAvailable(providerName, identity);

            // This method is used in the auth workflow. If the username is taken it will add a number
            // to the user name.
            if (UserExists(userName))
            {
                throw new InvalidOperationException(String.Format("{0} is already in use.", userName));
            }

            var user = new EmailUser
            {
                Name = userName,
                Status = 1,
                LastActivity = DateTime.UtcNow
            };

            var emailUserIdentity = new EmailUserIdentity
            {
                Key = Guid.NewGuid(),
                User = user,
                Email = email,
                Identity = identity,
                ProviderName = providerName
            };
            _session.Store(user);
            _session.Store(emailUserIdentity);
            _session.SaveChanges();

            return user;
        }


        public void LinkIdentity(EmailUser user, ClaimsPrincipal claimsPrincipal)
        {
            var identity = claimsPrincipal.GetClaimValue(ClaimTypes.NameIdentifier);
            var email = claimsPrincipal.GetClaimValue(ClaimTypes.Email);
            var providerName = claimsPrincipal.GetIdentityProvider();

            // Link this new identity
            user.Identities.Add(new EmailUserIdentity
            {
                Email = email,
                Identity = identity,
                ProviderName = providerName
            });
        }

        public EmailUser AddUser(string userName, string email, string password)
        {
            if (!IsValidUserName(userName))
            {
                throw new InvalidOperationException(String.Format("{0} is not a valid user name.", userName));
            }

            if (String.IsNullOrEmpty(password))
            {
                ThrowPasswordIsRequired();
            }

            EnsureUserNameIsAvailable(userName);

            var user = new EmailUser
            {
                Name = userName,
                Email = email,
                Status = 1,
                Salt = _crypto.CreateSalt(),
                LastActivity = DateTime.UtcNow
            };

            ValidatePassword(password);
            user.Password = password.ToSha256(user.Salt);

            _session.Store(user);
            _session.SaveChanges();

            return user;
        }

        public bool TryAuthenticateUser(string userName, string password, out EmailUser user)
        {
            user = _session.Query<EmailUser>().FirstOrDefault(x => x.Name == userName && x.Password == password.ToSha256(x.Salt));

            return user != null;
        }

        public void ChangeUserName(EmailUser user, string newUserName)
        {
            if (!IsValidUserName(newUserName))
            {
                throw new InvalidOperationException(String.Format("{0} is not a valid user name.", newUserName));
            }

            if (user.Name.Equals(newUserName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("That's already your username...");
            }

            EnsureUserNameIsAvailable(newUserName);

            // Update the user name
            user.Name = newUserName;
        }

        public void SetUserPassword(EmailUser user, string password)
        {
            ValidatePassword(password);
            user.Password = password.ToSha256(user.Salt);
        }

        public void ChangeUserPassword(EmailUser user, string oldPassword, string newPassword)
        {
            if (user.Password != oldPassword.ToSha256(user.Salt))
            {
                throw new InvalidOperationException("Passwords don't match.");
            }

            ValidatePassword(newPassword);

            EnsureSaltedPassword(user, newPassword);
        }

        public void RequestResetPassword(EmailUser user, int requestValidThroughInHours)
        {
            user.RequestPasswordResetId = HttpServerUtility.UrlTokenEncode(_crypto.CreateToken(user.Name));
            user.RequestPasswordResetValidThrough = DateTimeOffset.UtcNow.AddHours(requestValidThroughInHours);
        }

        public void ResetUserPassword(EmailUser user, string newPassword)
        {
            user.RequestPasswordResetId = null;
            user.RequestPasswordResetValidThrough = null;

            ValidatePassword(newPassword);

            EnsureSaltedPassword(user, newPassword);
        }

        public string GetUserNameFromToken(string token)
        {
            try
            {
                var decodedToken = HttpServerUtility.UrlTokenDecode(token);
                return _crypto.GetValueFromToken(decodedToken);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static void ValidatePassword(string password)
        {
            if (String.IsNullOrEmpty(password) || password.Length < passwordMinLength)
            {
                throw new InvalidOperationException(String.Format("Your password must be at least {0} characters.", passwordMinLength));
            }
        }

        private static bool IsValidUserName(string name)
        {
            return !String.IsNullOrEmpty(name) && Regex.IsMatch(name, "^[\\w-_.]{1,30}$");
        }

        private void EnsureSaltedPassword(EmailUser user, string password)
        {
            if (String.IsNullOrEmpty(user.Salt))
            {
                user.Salt = _crypto.CreateSalt();
            }
            user.Password = password.ToSha256(user.Salt);
        }

        private void EnsureUserNameIsAvailable(string userName)
        {
            if (UserExists(userName))
            {
                ThrowUserExists(userName);
            }
        }

        private bool UserExists(string userName)
        {
            return _session.Query<EmailUser>().Any(u => u.Name == userName);
        }

        private void EnsureProviderAndIdentityAvailable(string providerName, string identity)
        {
            if (ProviderAndIdentityExist(providerName, identity))
            {
                ThrowProviderAndIdentityExist(providerName, identity);
            }
        }

        private bool ProviderAndIdentityExist(string providerName, string identity)
        {
            return
                _session.Query<EmailUserIdentity>().Where(u => u.Identity == identity && u.ProviderName == providerName) !=
                null;
        }

        internal static string NormalizeUserName(string userName)
        {
            return userName.StartsWith("@") ? userName.Substring(1) : userName;
        }

        internal static void ThrowUserExists(string userName)
        {
            throw new InvalidOperationException(String.Format("Username {0} already taken.", userName));
        }

        internal static void ThrowPasswordIsRequired()
        {
            throw new InvalidOperationException("A password is required.");
        }

        internal static void ThrowProviderAndIdentityExist(string providerName, string identity)
        {
            throw new InvalidOperationException(String.Format("Identity {0} already taken with Provider {1}, please login with a different provider/identity combination.", identity, providerName));
        }
    }
}