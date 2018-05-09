﻿using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using LiteDB;

namespace LiteDB
{
    /// <summary>
    /// Encryption AES wrapper to encrypt data pages
    /// </summary>
    internal class AesEncryption
    {
        private Aes _aes;

        public AesEncryption(string password, byte[] salt)
        {
            _aes = Aes.Create();
            _aes.Padding = PaddingMode.Zeros;
            Rfc2898DeriveBytes pdb = null;

            try
            {
                pdb = new Rfc2898DeriveBytes(password, salt);
                _aes.Key = pdb.GetBytes(32);
                _aes.IV = pdb.GetBytes(16);
            }
            finally
            {
                IDisposable disp = pdb as IDisposable;

                if (disp != null)
                {
                    disp.Dispose();
                }
            }
        }

        /// <summary>
        /// Encrypt byte array returning new encrypted byte array with same length of original array (PAGE_SIZE)
        /// </summary>
        public byte[] Encrypt(byte[] bytes)
        {
            using (var encryptor = _aes.CreateEncryptor())
            using (var stream = new MemoryStream())
            using (var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                crypto.Write(bytes, 0, bytes.Length);
                crypto.FlushFinalBlock();
                stream.Position = 0;
                var encrypted = new byte[stream.Length];
                stream.Read(encrypted, 0, encrypted.Length);

                return encrypted;
            }
        }

        /// <summary>
        /// Decrypt and byte array returning a new byte array
        /// </summary>
        public byte[] Decrypt(byte[] encryptedValue)
        {
            using (var decryptor = _aes.CreateDecryptor())
            using (var stream = new MemoryStream())
            using (var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
            {
                crypto.Write(encryptedValue, 0, encryptedValue.Length);
                crypto.FlushFinalBlock();
                stream.Position = 0;
                var decryptedBytes = new Byte[stream.Length];
                stream.Read(decryptedBytes, 0, decryptedBytes.Length);

                return decryptedBytes;
            }
        }

        /// <summary>
        /// Hash a password using SHA1 just to verify password
        /// </summary>
        public static byte[] HashSHA1(string password)
        {
            var sha = SHA1.Create();
            var shaBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return shaBytes;
        }

        /// <summary>
        /// Generate a salt key that will be stored inside first page database
        /// </summary>
        /// <returns></returns>
        public static byte[] Salt(int maxLength = 16)
        {
#if NET35
            var random = new RNGCryptoServiceProvider();

            // empty salt array
            var salt = new byte[maxLength];

            // build the random bytes
            random.GetNonZeroBytes(salt);

            // Return the string encoded salt
            return salt;
#else
            // simple solution for NETStandard
            var rnd = new Random();
            var salt = new byte[maxLength];

            rnd.NextBytes(salt);

            return salt;
#endif
        }

        public void Dispose()
        {
            if (_aes != null)
            {
                _aes = null;
            }
        }
    }
}