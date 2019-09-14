using System;
using System.Security.Cryptography;
using System.Text;

public class MD5Helper
{
    public static string Encrypt(string plainText)
    {
        UTF8Encoding encoding1 = new UTF8Encoding();
        MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
        byte[] buffer1 = encoding1.GetBytes(plainText);
        byte[] buffer2 = provider1.ComputeHash(buffer1);
        return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
    }
}
