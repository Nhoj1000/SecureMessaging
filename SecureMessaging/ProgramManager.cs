// Author: John Kudela

using SecureMessaging.Model;

namespace SecureMessaging;

/// <summary>
/// The heart of the program, it has all the functions that the user can do and
/// feeds them to the correct utility classes then notifies user of process status
/// </summary>
public class ProgramManager
{
    private readonly Random _rng = new();

    private readonly MessagingRestClient _restClient = new();
    private readonly EncryptionUtility _encryptionUtility = new();
    private readonly KeyStorage _keyStorage = new();
    private readonly PrimeFinder _primeFinder = new();

    /// <summary>
    /// Generates and saves a private/public key pair locally
    /// </summary>
    /// <param name="keysize">number of bits in N</param>
    public void KeyGen(int keysize)
    {
        // random skew between .2 and .3 in either the positive or negative direction
        var skew = (_rng.Next(20, 31) * (_rng.Next() % 2 == 0 ? -1 : 1)) / 100f;
        var skewedBitIndex = (int)Math.Round(keysize * (0.5 + skew));

        // get p and q
        var p = _primeFinder.FindPrime(skewedBitIndex);
        var q = _primeFinder.FindPrime(keysize - skewedBitIndex);

        // determine all key values
        var N = p * q;
        var r = (p - 1) * (q - 1);
        var E = _primeFinder.FindPrime(16);
        var D = EncryptionUtility.ModInverse(E, r);

        // generate keys
        var publicKeyContent = _encryptionUtility.GenerateKeyFromComponents(E, N);
        var privateKeyContent = _encryptionUtility.GenerateKeyFromComponents(D, N);

        // save keys locally
        _keyStorage.SetPrivateKey(new PrivateKey(Array.Empty<string>(), privateKeyContent));
        _keyStorage.SetMyPublicKey(new PublicKey("", publicKeyContent));
    }

    /// <summary>
    /// Sends the local private key to a given email on the CS server
    /// </summary>
    /// <param name="email">The email to send to</param>
    public void SendKey(string email)
    {
        var publicKey = _keyStorage.GetMyPublicKey();
        var privateKey = _keyStorage.GetPrivateKey();
        if (publicKey == null || privateKey == null)
        {
            Console.WriteLine("No key exists locally, generate keys first");
        }
        else
        {
            publicKey.Email = email;
            if (_restClient.PutKey(publicKey))
            {
                // update email list in private key
                var emails = new HashSet<string>(privateKey.Email) { email };
                privateKey.Email = emails.ToArray();
                _keyStorage.SetPrivateKey(privateKey);
                Console.WriteLine("Key saved");
            }
            else
            {
                Console.WriteLine($"Unable to save key to server for {email}");
            }
        }
    }

    /// <summary>
    /// Retrieves a public key from the CS server
    /// </summary>
    /// <param name="email">The email to grab the key from</param>
    public void GetKey(string email)
    {
        var key = _restClient.GetKey(email);
        if (key == null)
        {
            Console.WriteLine($"Unable to retrieve key from server for {email}");
        }
        else if (!_keyStorage.SetOtherPublicKey(key))
        {
            Console.WriteLine($"Unable to save key locally for {email}");
        }
    }

    /// <summary>
    /// Send an encrypted message to an email on the CS server
    /// </summary>
    /// <param name="email">The email to send the message to</param>
    /// <param name="message">The plaintext message to send</param>
    public void SendMsg(string email, string message)
    {
        var userKey = _keyStorage.GetOtherPublicKey(email);
        if (userKey == null)
        {
            Console.WriteLine($"Key does not exist for {email}");
        }
        else
        {
            var encodedMsg = _encryptionUtility.EncryptMessage(message, userKey.Key);
            if (encodedMsg == null)
            {
                Console.WriteLine("Message is too long");
            }
            else
            {
                Console.WriteLine(_restClient.PutMessage(new Message(email, encodedMsg))
                    ? "Message written"
                    : $"Unable to send message to server for {email}");
            }
        }
    }

    /// <summary>
    /// Gets a message from the server and decrypts it with the local private key
    /// </summary>
    /// <param name="email">The email to grab the message of</param>
    public void GetMsg(string email)
    {
        var myKey = _keyStorage.GetPrivateKey();
        if (myKey == null)
        {
            Console.WriteLine("No private key exists locally");
        }
        else if (!myKey.Email.Contains(email))
        {
            Console.WriteLine($"No private key exists locally for {email}");
        }
        else
        {
            var encryptedMessage = _restClient.GetMessage(email);
            Console.WriteLine(encryptedMessage == null
                ? $"Unable to find message on server for email {email}"
                : _encryptionUtility.DecryptMessage(encryptedMessage.Content, myKey.Key));
        }
    }
}