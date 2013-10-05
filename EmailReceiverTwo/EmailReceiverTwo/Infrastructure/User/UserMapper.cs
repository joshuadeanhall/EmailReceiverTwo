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
        private IDocumentSession _documentSession;

        public UserMapper(IDocumentSession documentSession)
        {
            _documentSession = documentSession;

        } 

        public Guid? ValidateUser(string username, string password)
        {
            var userRecord = _documentSession.Query<EmailUser>().FirstOrDefault(x => x.Name == username && x.Password == EncodePassword(password));

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
            var userRecord = new EmailUser()
            {
                Id = Guid.NewGuid(),
                LoginType = "Default",
                Email = model.Email,
                FriendlyName = model.Name,
                Name = model.UserName,
                Password = EncodePassword(model.Password)
            };

            var existingUser = _documentSession.Query<EmailUser>().FirstOrDefault(x => x.Email == userRecord.Email && x.LoginType == "Default");
            if (existingUser != null)
                return null;

            _documentSession.Store(userRecord);
            _documentSession.SaveChanges();

            return userRecord.Id;
        }
    }
}