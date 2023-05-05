namespace ProjectAspoeck;

public static class EncryptionHelper
{
  public static byte[] GenerateSalt()
  {
    // Generate a 16-byte (128-bit) salt value
    byte[] salt = new byte[16];
    using (var rng = new RNGCryptoServiceProvider())
    {
      rng.GetBytes(salt);
    }
    return salt;
  }
  private static readonly byte[] Salt = GenerateSalt();

  private const int Iterations = 10000;
  private const int KeySize = 256;

  public static string Encrypt(string plainText, string password)
  {
    
    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
    byte[] keyBytes = new Rfc2898DeriveBytes(password, Salt, Iterations).GetBytes(KeySize / 8);

    using var aes = Aes.Create();
    aes.Mode = CipherMode.CBC;

    using var encryptor = aes.CreateEncryptor(keyBytes, aes.IV);
    using var ms = new MemoryStream();
    ms.Write(BitConverter.GetBytes(aes.IV.Length), 0, sizeof(int));
    ms.Write(aes.IV, 0, aes.IV.Length);

    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
    {
      cs.Write(plainBytes, 0, plainBytes.Length);
      cs.Close();
    }

    return Convert.ToBase64String(ms.ToArray());
  }

  public static string Decrypt(string cipherText, string password)
  {
    byte[] cipherBytes = Convert.FromBase64String(cipherText);
    byte[] keyBytes = new Rfc2898DeriveBytes(password, Salt, Iterations).GetBytes(KeySize / 8);

    using var aes = Aes.Create();
    aes.Mode = CipherMode.CBC;

    int ivLength = BitConverter.ToInt32(cipherBytes, 0);
    byte[] iv = new byte[ivLength];
    Array.Copy(cipherBytes, sizeof(int), iv, 0, ivLength);

    using var decryptor = aes.CreateDecryptor(keyBytes, iv);
    using var ms = new MemoryStream();
    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
    {
      cs.Write(cipherBytes, ivLength + sizeof(int), cipherBytes.Length - ivLength - sizeof(int));
      cs.Close();
    }

    return Encoding.UTF8.GetString(ms.ToArray());
  }
}
