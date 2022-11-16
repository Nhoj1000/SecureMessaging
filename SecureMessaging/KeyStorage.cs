// Author: John Kudela

using System.Text.Json;
using SecureMessaging.Model;

namespace SecureMessaging;

/*
 * This class manages local key storage
 */
public class KeyStorage
{
    // stores current working directory string
    private static readonly string Dir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;

    /// <summary>
    /// Returns the public key of another user if it's been stored
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <returns>The public key of the user, null otherwise</returns>
    public PublicKey? GetOtherPublicKey(string email)
    {
        try
        {
            var key = File.ReadAllText($"{Dir}{email}.key");
            return JsonSerializer.Deserialize<PublicKey>(key);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
    
    /// <summary>
    /// Returns the public key of this user if it's been generated
    /// </summary>
    /// <returns>The public key of this user, null otherwise</returns>
    public PublicKey? GetMyPublicKey()
    {
        try
        {
            var key = File.ReadAllText($"{Dir}public.key");
            return JsonSerializer.Deserialize<PublicKey>(key);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the private key of this user if it's been generated
    /// </summary>
    /// <returns>The private key of this user, null otherwise</returns>
    public PrivateKey? GetPrivateKey()
    {
        try
        {
            var key = File.ReadAllText($"{Dir}private.key");
            return JsonSerializer.Deserialize<PrivateKey>(key);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// Saves the public key of another user in local storage
    /// </summary>
    /// <param name="publicKey">The key to save</param>
    /// <returns>True if save was successful, false otherwise</returns>
    public bool SetOtherPublicKey(PublicKey publicKey)
    {
        try
        {
            File.WriteAllText($"{Dir}{publicKey.Email}.key", JsonSerializer.Serialize(publicKey));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Saves the public key of this user in local storage
    /// </summary>
    /// <param name="publicKey">The key to save</param>
    /// <returns>True if save was successful, false otherwise</returns>
    public bool SetMyPublicKey(PublicKey publicKey)
    {
        try
        {
            File.WriteAllText($"{Dir}public.key", JsonSerializer.Serialize(publicKey));
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    
    /// <summary>
    /// Saves the private key of this user in local storage
    /// </summary>
    /// <param name="privateKey">The key to save</param>
    /// <returns>True if save was successful, false otherwise</returns>
    public bool SetPrivateKey(PrivateKey privateKey)
    {
        try
        {
            File.WriteAllText($"{Dir}private.key", JsonSerializer.Serialize(privateKey));
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}