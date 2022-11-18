// Author: John Kudela

using System.Numerics;
using System.Text;

namespace SecureMessaging;

public class EncryptionUtility
{
    public string EncryptMessage(string msg, string publicKey)
    {
        var values = GetKeyComponents(publicKey);
        var trueE = values[0];
        var trueN = values[1];

        var msgBytes = Encoding.UTF8.GetBytes(msg);
        var plaintext = new BigInteger(msgBytes);

        var ciphertext = BigInteger.ModPow(plaintext, trueE, trueN);
        return Convert.ToBase64String(ciphertext.ToByteArray());
    }

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