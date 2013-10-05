using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EmailReceiver.Models;
using EmailReceiverTwo.Domain;
using Nancy;
using Nancy.Security;
using Raven.Client;
using Raven.Client.Linq;

namespace EmailReceiverTwo.Infrastructure.User
{
    public class UserMapper
    {
        private IDocumentSession DocumentSession;

        public UserMapper(IDocumentSession DocumentSession)
        {
            this.DocumentSession = DocumentSession;

        } 
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var userRecord = DocumentSession.Load<UserModel>(identifier);
            return userRecord == null ? null : new UserIdentity() { UserName = userRecord.Username, FriendlyName = userRecord.FriendlyName };
        }

        public Guid? ValidateUser(string username, string password)
        {
            var userRecord = DocumentSession.Query<UserModel>().Where(x => x.Username == username && x.Password == EncodePassword(password)).FirstOrDefault();

            if (userRecord == null)
            {
                return null;
            }
            return userRecord.Id;
        }

        public static string EncodePassword(string originalPassword)
        {
            if (originalPassword == null)
                return String.Empty;

            //Declarations

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            var md5 = new MD5CryptoServiceProvider();
            var originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            var encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        public Guid? ValidateRegisterNewUser(RegisterViewModel model)
        {
            var userRecord = new UserModel()
            {
                Id = Guid.NewGuid(),
                LoginType = "Default",
                EmailAddress = model.Email,
                FriendlyName = model.Name,
                Username = model.UserName,
                Password = EncodePassword(model.Password)
            };

            var existingUser = DocumentSession.Query<UserModel>().Where(x => x.EmailAddress == userRecord.EmailAddress && x.LoginType == "Default").FirstOrDefault();
            if (existingUser != null)
                return null;

            DocumentSession.Store(userRecord);
            DocumentSession.SaveChanges();

            return userRecord.Id;
        }
    }
}