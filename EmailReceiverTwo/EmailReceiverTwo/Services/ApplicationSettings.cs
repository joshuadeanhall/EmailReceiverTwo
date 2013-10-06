﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using EmailReceiverTwo.Infrastructure;

namespace EmailReceiverTwo.Services
{
    public class ApplicationSettings
    {
        public ApplicationSettings()
        {
            EncryptionKey = CryptoHelper.ToHex(GenerateRandomBytes());
            VerificationKey = CryptoHelper.ToHex(GenerateRandomBytes());
        }

        public string EncryptionKey { get; set; }

        public string VerificationKey { get; set; }

        public string AzureblobStorageConnectionString { get; set; }

        public int MaxFileUploadBytes { get; set; }

        public string GoogleAnalytics { get; set; }

        public bool AllowUserRegistration { get; set; }

        public bool AllowUserResetPassword { get; set; }

        public int RequestResetPasswordValidThroughInHours { get; set; }

        public bool AllowRoomCreation { get; set; }

        public IDictionary<string, string> AuthenticationProviders { get; set; }

        public string EmailSender { get; set; }

        public static ApplicationSettings GetDefaultSettings()
        {
            return new ApplicationSettings
            {
                EncryptionKey = CryptoHelper.ToHex(GenerateRandomBytes()),
                VerificationKey = CryptoHelper.ToHex(GenerateRandomBytes()),
                MaxFileUploadBytes = 5242880,
                AllowUserRegistration = true,
                AllowRoomCreation = true,
                AllowUserResetPassword = false,
                RequestResetPasswordValidThroughInHours = 6,
                EmailSender = String.Empty
            };
        }

        private static byte[] GenerateRandomBytes(int n = 32)
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[n];
                cryptoProvider.GetBytes(bytes);
                return bytes;
            }
        }
    }
}