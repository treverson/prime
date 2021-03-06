﻿using System;
using System.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Prime.Core.Encryption;
using Prime.Core;

namespace Prime.ConsoleApp.Tests.Frank
{
    public static class AuthManagerTest
    {
        public static void Key1(ServerContext context)
        {
            var logger = context.L;

            var password = "hello world!";
            var identity = "prime-user";

            var krgen = EncryptionHelper.GenerateKeyRingGenerator(new KeyRingParams(identity, password) { Length = 2048 });

            var pkr = krgen.GeneratePublicKeyRing();
            var pubout = new BufferedStream(new FileStream(@"c:\tmp\dummy.pkr", System.IO.FileMode.Create));
            pkr.Encode(pubout);
            pubout.Close();

            // Generate private key, dump to file.
            var skr = krgen.GenerateSecretKeyRing();
            var secout = new BufferedStream(new FileStream(@"c:\tmp\dummy.skr", System.IO.FileMode.Create));
            skr.Encode(secout);
            secout.Close();

            // Generate public key ring.
            var pubKey = EncryptionHelper.GetKeyString(pkr);

            // Generate private key.
            var privateKey = EncryptionHelper.GetKeyString(skr);

            logger.Log("Pub: " + pubKey);
            logger.Log("Priv: " + privateKey);
        }

        public static void EcdsaKeySign(ServerContext context)
        {
            var logger = context.L;
            var size = AsymmetricKeySize.S256;
            var s = "Hello World!";

            logger.Log("======= Key Size: {0} =======", size);
            try
            {
                var key = EncryptionHelper.GenerateKeys(size);
                var signature = EncryptionHelper.GetSignature(s, key);
                var signatureOk = EncryptionHelper.VerifySignature(key, s, signature);

                var pubicKey = (ECPublicKeyParameters)(key.Public);
                var privateKey = (ECPrivateKeyParameters)(key.Private);

                logger.Log("Input Text: " +s);
                logger.Log("Key ({0} bytes): {1}", privateKey.D.BitLength, privateKey.D);
                logger.Log("Signature ({0} bytes): {1}", signature.Length, signature.ToX2String());
                logger.Log("Signature verified: {0}", signatureOk);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}