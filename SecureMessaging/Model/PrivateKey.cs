namespace SecureMessaging;

public class PrivateKey
{
    public PrivateKey(string[] email, string key)
    {
        this.Email = email;
        this.Key = key;
    }

    public string[] Email { get; set; }
    public string Key { get; set; }
}