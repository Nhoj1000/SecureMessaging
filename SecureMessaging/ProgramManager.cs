// Author: John Kudela

namespace SecureMessaging;

/*
 * The heart of the program, it has all the functions that the user can do and
 * feeds them to the correct utility classes
 */
public class ProgramManager
{
    private static readonly MessagingRestClient RestClient = new();
    private static readonly KeyStorage KeyStorage = new();
    private static readonly PrimeFinder PrimeFinder = new();

    public bool KeyGen(int keysize)
    {
        return false;
    }

    public bool SendKey(string email)
    {
        return false;
    }

    public bool GetKey(string email)
    {
        var key = RestClient.GetKey(email);
        if (key == null) return false;
        KeyStorage.SetOtherPublicKey(key);
        return true;
    }

    public bool SendMsg(string email, string message)
    {
        return false;
    }

    public bool GetMsg(string email)
    {
        return false;
    }
}


// var key = Client.GetKey(email);
// Console.WriteLine(key?.Email);
// Console.WriteLine(key?.Key);

// var message = Client.GetMessage(email);
// Console.WriteLine(message?.Email);
// Console.WriteLine(message?.Content);
//
// var success = Client.PutKey(new PublicKey(email, "testKey4"));
// Console.WriteLine(success);
//
// success = Client.PutMessage(new Message(email, "testMessage4"));
// Console.WriteLine(success);
//
// key = Client.GetKey(email);
// Console.WriteLine(key?.Email);
// Console.WriteLine(key?.Key);
//
// message = Client.GetMessage(email);
// Console.WriteLine(message?.Email);
// Console.WriteLine(message?.Content);
//
//
// Console.WriteLine(Storage.SetOtherPublicKey(key));
// var fileKey = Storage.GetOtherPublicKey(email);
// Console.WriteLine("file: " + fileKey?.Email);
// Console.WriteLine("file: " + fileKey?.Key);