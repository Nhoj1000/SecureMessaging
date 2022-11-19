// Author: John Kudela

using System.Numerics;
using System.Text;

namespace SecureMessaging;

/// <summary>
/// Core utility class for encrypting/decrypting mesages
/// </summary>
public class EncryptionUtility
{
    /// <summary>
    /// Encrypts a message given a public key
    /// </summary>
    /// <param name="msg">The plaintext message to send</param>
    /// <param name="publicKey">The public key</param>
    /// <returns>Base 64 encrypted string or null if msg is too big</returns>
    public string? EncryptMessage(string msg, string publicKey)
    {
        var values = GetKeyComponents(publicKey);
        var trueE = values[0];
        var trueN = values[1];

        var msgBytes = Encoding.UTF8.GetBytes(msg);
        var plaintext = new BigInteger(msgBytes);

        if (plaintext > (trueN - 1)) return null;

        var ciphertext = BigInteger.ModPow(plaintext, trueE, trueN);
        return Convert.ToBase64String(ciphertext.ToByteArray());
    }

    /// <summary>
    /// Decrypts a message
    /// </summary>
    /// <param name="msg">The base 64 encrypted string</param>
    /// <param name="privateKey">The private key to decode the message</param>
    /// <returns>The decoded message</returns>
    public string DecryptMessage(string msg, string privateKey)
    {
        var values = GetKeyComponents(privateKey);
        var trueD = values[0];
        var trueN = values[1];

        var msgBytes = Convert.FromBase64String(msg);
        var ciphertext = new BigInteger(msgBytes);

        var plaintext = BigInteger.ModPow(ciphertext, trueD, trueN);
        return Encoding.UTF8.GetString(plaintext.ToByteArray());
    }

    /// <summary>
    /// Helper method to generate a key given an E/D and an N value
    /// </summary>
    /// <param name="first">E or D value</param>
    /// <param name="n">N value</param>
    /// <returns>Base 64 string with the key in it</returns>
    public string GenerateKeyFromComponents(BigInteger first, BigInteger n)
    {
        var ans = new List<byte>();

        var firstCountBytes = BitConverter.GetBytes(first.GetByteCount());
        if (BitConverter.IsLittleEndian) Array.Reverse(firstCountBytes);
        ans.AddRange(firstCountBytes);

        ans.AddRange(first.ToByteArray(true));

        var nCountBytes = BitConverter.GetBytes(n.GetByteCount());
        if (BitConverter.IsLittleEndian) Array.Reverse(nCountBytes);
        ans.AddRange(nCountBytes);

        ans.AddRange(n.ToByteArray(false));

        return Convert.ToBase64String(ans.ToArray());
    }

    /// <summary>
    /// Breaks an encoded key down into an E/D and N
    /// </summary>
    /// <param name="key">The base 64 encoded key string</param>
    /// <returns>An array with E/D in index 0 and N in index 1</returns>
    private static BigInteger[] GetKeyComponents(string key)
    {
        var keyBytes = Convert.FromBase64String(key);

        // get e/d and make sure it's big endian
        var firstBytes = keyBytes[..4];
        if (BitConverter.IsLittleEndian) Array.Reverse(firstBytes);
        var first = BitConverter.ToInt32(firstBytes);

        // read E/D as e/d bytes and ensure little endian
        var trueFirstBytes = keyBytes[4..(4 + first)];
        if (!BitConverter.IsLittleEndian) Array.Reverse(trueFirstBytes);
        var trueFirst = new BigInteger(trueFirstBytes);

        // get n and ensure big endian
        var nBytes = keyBytes[(4 + first)..(4 + first + 4)];
        if (BitConverter.IsLittleEndian) Array.Reverse(nBytes);
        var n = BitConverter.ToInt32(nBytes);

        // get N and ensure little endian
        var trueNBytes = keyBytes[(4 + first + 4)..(4 + first + 4 + n)];
        if (!BitConverter.IsLittleEndian) Array.Reverse(trueNBytes);
        var trueN = new BigInteger(trueNBytes);

        BigInteger[] values = { trueFirst, trueN };
        return values;
    }

    /// <summary>
    /// Modinverse method for finding a D given an E and phi(n)
    /// </summary>
    /// <param name="a">E value</param>
    /// <param name="n">phi(n)</param>
    /// <returns></returns>
    public static BigInteger ModInverse(BigInteger a, BigInteger n)
    {
        BigInteger i = n, v = 0, d = 1;
        while (a > 0)
        {
            BigInteger t = i / a, x = a;
            a = i % x;
            i = x;
            x = d;
            d = v - t * x;
            v = x;
        }

        v %= n;
        if (v < 0) v = (v + n) % n;
        return v;
    }
}