//Author: John Kudela

using System.Numerics;
using System.Security.Cryptography;

namespace SecureMessaging;

/// <summary>
/// Extension class to BigInt that adds an "isProbablyPrime()" method
/// </summary>
public static class BigIntExtension
{
    //used to prune out large composite numbers quicker
    private static readonly int[] FirstPrimesUnder1000 = { 2,  3,  5,  7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53,
        59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
        179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
        307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433,
        439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577,
        587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719,
        727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863,
        877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997 };
    
    /// <summary>
    /// Uses the Miller-Rabin algorithm to run a probably-prime check on a BigInteger
    /// </summary>
    /// <param name="value">Value to check primality of</param>
    /// <param name="k">The number of witness checks you want to run, default is 10</param>
    /// <returns>True if the value is probably prime</returns>
    public static Boolean IsProbablyPrime(this BigInteger value, int k = 10)
    {
        //make sure
        if (FirstPrimesUnder1000.Any(i => i == value))
        {
            return true;
        }
        if (value <= 3 || FirstPrimesUnder1000.Any(i => value % i == 0 && value != i))
        {
            return false;
        }

        //get r and d such that (2^r)*d + 1 = value for the greatest possible r
        var r = 0;
        var d = value - 1;
        var tempR = r;
        var tempD = d;
        while ((2 ^ tempR) * tempD + 1 == value)
        {
            r = tempR;
            d = tempD;
            tempR += 1;
            tempD /= 2;
        }

        //witness loop
        for (var i = 0; i < k; i++)
        {
            var a = RandBigInt(value - 2);
            var x = BigInteger.ModPow(a, d, value);
            if (x == 1 || x == value - 1)
            {
                continue;
            }
            var found = false;
            for (var j = 0; j < r - 1; j++)
            {
                x = BigInteger.ModPow(x, 2, value);
                if (x != value - 1) continue;
                found = true;
                break;
            }

            if (found)
            {
                continue;
            }
            return false;
        }

        return true;
    }

    //generates a random BigInteger between 3 and the highLimit
    private static BigInteger RandBigInt(BigInteger highLimit)
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[highLimit.GetByteCount()];
        BigInteger bi;

        do
        {
            rng.GetBytes(bytes);
            bi = new BigInteger(bytes);
            bi %= highLimit;
        } while (bi < 3);
        return bi;
    }
}