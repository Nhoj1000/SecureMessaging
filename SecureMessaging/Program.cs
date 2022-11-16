// Author: John Kudela

/*
 * Project 2 - Secure Messaging
 *
 * Allows for encoding/decoding of private keys for
 * communication via the RIT CS server
 */

namespace SecureMessaging
{
    public class SecureMessaging
    {
        private static PrimeFinder _primeFinder = new PrimeFinder();
        private static readonly MessagingRestClient Client = new MessagingRestClient();
        private static readonly KeyManager _manager = new KeyManager();

        private static readonly string email = "jwk1817@rit.edu";

        static async Task Main(string[] args)
        {
            
            // var key = await Client.GetKey(email);
            // Console.WriteLine(key?.Email);
            // Console.WriteLine(key?.Key);
            //
            // var message = await Client.GetMessage(email);
            // Console.WriteLine(message?.Email);
            // Console.WriteLine(message?.Content);
            //
            // var success = await Client.PutKey(new PublicKey(email, "testKey3"));
            // Console.WriteLine(success);
            //
            // success = await Client.PutMessage(new Message(email, "testMessage3"));
            // Console.WriteLine(success);
            //
            // key = await Client.GetKey(email);
            // Console.WriteLine(key?.Email);
            // Console.WriteLine(key?.Key);
            //
            // message = await Client.GetMessage(email);
            // Console.WriteLine(message?.Email);
            // Console.WriteLine(message?.Content);
            //
            //
            // Console.WriteLine(_manager.SetOtherPublicKey(key));
            // var fileKey = _manager.GetOtherPublicKey(email);
            // Console.WriteLine("file: " + fileKey?.Email);
            // Console.WriteLine("file: " + fileKey?.Key);
        }
    }
}