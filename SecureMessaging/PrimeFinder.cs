//Author: John Kudela

using System.Numerics;
using System.Security.Cryptography;

namespace SecureMessaging;

/// <summary>
/// Runs a parallel search on all available CPU cores, generating
/// random numbers and checking if they're probable primes using
/// the Miller-Rabin algorithm. Returns a prime once found
/// </summary>
public class PrimeFinder
{
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    /// <summary>
    /// Runs a parallel search on all available CPU cores, generating
    /// random numbers and checking if they're probable primes using
    /// the Miller-Rabin algorithm. Returns one once found
    /// </summary>
    public BigInteger FindPrime(int numBytes)
    {
        var found = false;
        BigInteger probablePrime = -1;
        //make as many threads as there are logical processors
        Parallel.ForEach(Enumerable.Range(1, Environment.ProcessorCount), (_ =>
        {
            while (!found)
            {
                //otherwise generate new prime and check it
                var bytes = new byte[numBytes];
                _rng.GetBytes(bytes);
                var bi = new BigInteger(bytes);
                if (!bi.IsProbablyPrime()) continue;
                found = true;
                probablePrime = bi;
            }
        }));
        return probablePrime;
    }
}