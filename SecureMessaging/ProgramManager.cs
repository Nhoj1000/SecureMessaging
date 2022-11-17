// Author: John Kudela

using SecureMessaging.Model;

namespace SecureMessaging;

/*
 * The heart of the program, it has all the functions that the user can do and
 * feeds them to the correct utility classes then notifies user of process status
 */
public class ProgramManager
{
    private readonly MessagingRestClient _restClient = new();
    private readonly EncryptionUtility _encryptionUtility = new();
    private readonly KeyStorage _keyStorage = new();
    private readonly PrimeFinder _primeFinder = new();

    public void KeyGen(int keysize)
    {
    }

    public void SendKey(string email)
    {
        var publicKey = _keyStorage.GetMyPublicKey();
        if (publicKey == null)
        {
            Console.WriteLine("No private key exists locally");
        }
        else
        {
            publicKey.Email = email;
            Console.WriteLine(_restClient.PutKey(publicKey)
                ? "Key saved"
                : $"Unable to save key to server for ${email}");
        }
    }

    public void GetKey(string email)
    {
        var key = _restClient.GetKey(email);
        if (key == null)
        {
            Console.WriteLine($"Unable to retrieve key from server for ${email}");
        }
        else if (!_keyStorage.SetOtherPublicKey(key))
        {
            Console.WriteLine($"Unable to save key locally for ${email}");
        }
    }

    public void SendMsg(string email, string message)
    {
        var userKey = _keyStorage.GetOtherPublicKey(email);
        if (userKey == null)
        {
            Console.WriteLine($"Key does not exist for ${email}");
        }
        else
        {
            var encodedMsg = new Message(email, _encryptionUtility.EncryptMessage(message, userKey.Key));
            Console.WriteLine(_restClient.PutMessage(encodedMsg)
                ? "Message written"
                : $"Unable to send message to server for ${email}");
        }
    }

    public void GetMsg(string email)
    {
        var myKey = _keyStorage.GetPrivateKey();
        if (myKey == null)
        {
            Console.WriteLine("No private key exists locally");
        }
        else if (!myKey.Email.Contains(email))
        {
            Console.WriteLine($"No private key exists locally for ${email}");
        }
        else
        {
            var encryptedMessage = _restClient.GetMessage(email);
            Console.WriteLine(encryptedMessage == null
                ? $"Unable to find message on server for email ${email}"
                : _encryptionUtility.DecryptMessage(encryptedMessage.Content, myKey.Key));
        }
    }
}