// Author: John Kudela

namespace SecureMessaging.Model;

/*
 * Stores your public key locally
 */
public class PublicKey
{
    public PublicKey(string email, string key)
    {
        this.Email = email;
        this.Key = key;
    }

    public string Email { get; set; }
    public string Key { get; set; }
}