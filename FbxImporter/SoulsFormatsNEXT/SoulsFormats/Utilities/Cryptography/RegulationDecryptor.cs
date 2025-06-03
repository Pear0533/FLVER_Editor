using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System;

namespace SoulsFormats.Cryptography
{
    /// <summary>
    /// Helper methods for decrypting and encrypting regulations.
    /// </summary>
    public static class RegulationDecryptor
    {
        /// <summary>
        /// The different known regulation keys.
        /// </summary>
        public enum RegulationKey
        {
            /// <summary>
            /// The key for Dark Souls 3.
            /// </summary>
            DarkSouls3 = 0,

            /// <summary>
            /// The key for Elden Ring.
            /// </summary>
            EldenRing = 1,

            /// <summary>
            /// The key for Armored Core 6.
            /// </summary>
            ArmoredCore6 = 2,
            
            
            /// <summary>
            /// The key for ELDEN RING Nightreign.
            /// </summary>
            EldenRingNightreign = 3,
        }

        private static readonly Dictionary<RegulationKey, byte[]> RegulationKeyDictionary = new Dictionary<RegulationKey, byte[]>
            {
                { RegulationKey.DarkSouls3, SFEncoding.ASCII.GetBytes("ds3#jn/8_7(rsY9pg55GFN7VFL#+3n/)") },
                { RegulationKey.EldenRing, new byte[] { 0x99, 0xBF, 0xFC, 0x36, 0x6A, 0x6B, 0xC8, 0xC6, 0xF5, 0x82, 0x7D, 0x09, 0x36, 0x02, 0xD6, 0x76, 0xC4, 0x28, 0x92, 0xA0, 0x1C, 0x20, 0x7F, 0xB0, 0x24, 0xD3, 0xAF, 0x4E, 0x49, 0x3F, 0xEF, 0x99 } },
                { RegulationKey.ArmoredCore6, new byte[] { 0x10, 0xCE, 0xED, 0x47, 0x7B, 0x7C, 0xD9, 0xD7, 0xE6, 0x93, 0x8E, 0x11, 0x47, 0x13, 0xE7, 0x87, 0xD5, 0x39, 0x13, 0xB1, 0xD, 0x31, 0x8E, 0xC1, 0x35, 0xE4, 0xBE, 0x50, 0x50, 0x4E, 0xE, 0x10 } },
                { RegulationKey.EldenRingNightreign, new byte[] { 0x9A, 0x8E, 0xE9, 0x0C, 0x4C, 0x01, 0xA4, 0x31, 0x68, 0xA1, 0x7D, 0x9D, 0x75, 0xE4, 0xA7, 0xD0, 0x21, 0x07, 0xEB, 0xCF, 0x43, 0xD5, 0xAC, 0xB0, 0x55, 0x4F, 0x94, 0x16, 0x01, 0xB5, 0x79, 0x18 } }
            };

        /// <summary>
        /// Decrypts and unpacks DS3's regulation BND4 from the specified path.
        /// </summary>
        public static BND4 DecryptDS3Regulation(string path)
        {
            return DecryptBndWithKey(path, RegulationKey.DarkSouls3);
        }

        /// <summary>
        /// Decrypts and unpacks ER's regulation BND4 from the specified path.
        /// </summary>
        public static BND4 DecryptERRegulation(string path)
        {
            return DecryptBndWithKey(path, RegulationKey.EldenRing);
        }   

        /// <summary>
        /// Decrypts and unpacks AC6's regulation BND4 from the specified path.
        /// </summary>
        public static BND4 DecryptAC6Regulation(string path)
        {
            return DecryptBndWithKey(path, RegulationKey.ArmoredCore6);
        }
        
        /// <summary>
        /// Decrypts and unpacks ER Nightreign's regulation BND4 from the specified path.
        /// </summary>
        public static BND4 DecryptERNRRegulation(string path)
        {
            return DecryptBndWithKey(path, RegulationKey.EldenRingNightreign);
        }

        /// <summary>
        /// Decrypts and unpacks a regulation BND4 from the specified path with a provided key.
        /// </summary>
        public static BND4 DecryptBndWithKey(string path, RegulationKey key)
        {
            byte[] bytes = File.ReadAllBytes(path);
            bytes = DecryptByteArray(key, bytes);
            return BND4.Read(bytes);
        }

        /// <summary>
        /// Repacks and encrypts DS3's regulation BND4 to the specified path.
        /// </summary>
        public static void EncryptDS3Regulation(string path, BND4 bnd)
        {
            EncryptRegulationWithKey(path, bnd, RegulationKey.DarkSouls3);
        }

        /// <summary>
        /// Repacks and encrypts ER's regulation BND4 to the specified path.
        /// </summary>
        public static void EncryptERRegulation(string path, BND4 bnd)
        {
            EncryptRegulationWithKey(path, bnd, RegulationKey.EldenRing);
        }

        /// <summary>
        /// Repacks and encrypts AC6's regulation BND4 to the specified path.
        /// </summary>
        public static void EncryptAC6Regulation(string path, BND4 bnd)
        {
            EncryptRegulationWithKey(path, bnd, RegulationKey.ArmoredCore6);
        }
        
        /// <summary>
        /// Repacks and encrypts ER Nightreign's regulation BND4 to the specified path.
        /// </summary>
        public static void EncryptERNRRegulation(string path, BND4 bnd)
        {
            EncryptRegulationWithKey(path, bnd, RegulationKey.EldenRing);
        }

        /// <summary>
        /// Encrypts and writes a regulation BND4 from the specified path with a provided key.
        /// </summary>
        public static void EncryptRegulationWithKey(string path, BND4 bnd, RegulationKey key)
        {
            byte[] bytes = bnd.Write();
            bytes = EncryptByteArray(key, bytes);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, bytes);
        }

        private static byte[] EncryptByteArray(RegulationKey key, byte[] secret)
        {
            using (MemoryStream ms = new MemoryStream())
            using (AesManaged cryptor = new AesManaged())
            {
                cryptor.Mode = CipherMode.CBC;
                cryptor.Padding = PaddingMode.PKCS7;
                cryptor.KeySize = 256;
                cryptor.BlockSize = 128;

                byte[] iv = cryptor.IV;

                using (CryptoStream cs = new CryptoStream(ms, cryptor.CreateEncryptor(RegulationKeyDictionary[key], iv), CryptoStreamMode.Write))
                {
                    cs.Write(secret, 0, secret.Length);
                }

                byte[] encryptedContent = ms.ToArray();

                byte[] result = new byte[iv.Length + encryptedContent.Length];

                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);

                return result;
            }
        }

        private static byte[] DecryptByteArray(RegulationKey key, byte[] secret)
        {
            byte[] iv = new byte[16];
            byte[] encryptedContent = new byte[secret.Length - 16];

            Buffer.BlockCopy(secret, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(secret, iv.Length, encryptedContent, 0, encryptedContent.Length);

            using (MemoryStream ms = new MemoryStream())
            using (AesManaged cryptor = new AesManaged())
            {
                cryptor.Mode = CipherMode.CBC;
                cryptor.Padding = PaddingMode.None;
                cryptor.KeySize = 256;
                cryptor.BlockSize = 128;

                using (CryptoStream cs = new CryptoStream(ms, cryptor.CreateDecryptor(RegulationKeyDictionary[key], iv), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedContent, 0, encryptedContent.Length);
                }

                return ms.ToArray();
            }
        }
    }
}
