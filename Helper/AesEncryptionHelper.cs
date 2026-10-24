using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class AesEncryptionHelper
{
    private const string EmptyPasswordMarker = "__EMPTY_PASS__";

    public static string Encrypt(string plainText)
    {
        if (plainText == null) plainText = string.Empty;
        if (plainText.Length == 0) plainText = EmptyPasswordMarker;

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);

        return Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return string.Empty;

        try
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] plainBytes = ProtectedData.Unprotect(cipherBytes, null, DataProtectionScope.CurrentUser);

            string result = Encoding.UTF8.GetString(plainBytes);
            return result == EmptyPasswordMarker ? string.Empty : result;
        }
        catch
        {
            return string.Empty;
        }
    }
}
