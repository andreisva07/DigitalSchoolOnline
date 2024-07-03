using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class PasswordManager
{
    private static readonly string encryptionKey = "encryptionkeyeryrqn46hxjny";
    private static readonly string passwordsFilePath = "encryptedPasswords.txt";

    public static void SaveEncryptedPassword(string username, string plainTextPassword)
    {
        var encryptedPassword = EncryptString(plainTextPassword);
        var entry = $"{username}:{encryptedPassword}";
        File.AppendAllLines(passwordsFilePath, new[] { entry });
    }

    public static string GetDecryptedPassword(string username)
    {
        if (File.Exists(passwordsFilePath))
        {
            var entries = File.ReadAllLines(passwordsFilePath);
            foreach (var entry in entries)
            {
                var parts = entry.Split(':');
                if (parts[0] == username)
                {
                    return DecryptString(parts[1]);
                }
            }
        }
        return null;
    }

    private static string EncryptString(string plainText)
    {
        var key = Encoding.UTF8.GetBytes(encryptionKey);
        using (var aesAlg = Aes.Create())
        {
            using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
            {
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    var iv = aesAlg.IV;
                    var decryptedContent = msEncrypt.ToArray();
                    var result = new byte[iv.Length + decryptedContent.Length];
                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }
    }

    private static string DecryptString(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);
        var iv = new byte[16];
        var cipher = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
        var key = Encoding.UTF8.GetBytes(encryptionKey);
        using (var aesAlg = Aes.Create())
        {
            using (var decryptor = aesAlg.CreateDecryptor(key, iv))
            {
                using (var msDecrypt = new MemoryStream(cipher))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
