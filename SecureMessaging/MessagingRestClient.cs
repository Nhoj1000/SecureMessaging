using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SecureMessaging;

/*
 * REST client meant to handle all interactions with the CS server
 */
public class MessagingRestClient
{
    // http client to send requests
    private static readonly HttpClient Client = new HttpClient();

    // the ip of the server to hit
    private const string Ip = "http://kayrun.cs.rit.edu:5000";

    /// <summary>
    /// Gets the key for a given email
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <returns>A Task resolving to the PublicKey, or null if not found</returns> 
    public async Task<PublicKey?> GetKey(string email)
    {
        try
        {
            return await Client.GetFromJsonAsync<PublicKey>($"{Ip}/Key/{email}");
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the message for a given email
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <returns>A Task resolving to the Message or null if not found</returns>
    public async Task<Message?> GetMessage(string email)
    {
        try
        {
            return await Client.GetFromJsonAsync<Message>($"{Ip}/Message/{email}");
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Updates the key at a given email on the server
    /// </summary>
    /// <param name="key">The new PublicKey object to update</param>
    /// <returns>A task resolving to whether the put succeeded or not</returns>
    public async Task<bool> PutKey(PublicKey key)
    {
        var content = new StringContent(JsonSerializer.Serialize(key), Encoding.UTF8, "application/json");
        return (await Client.PutAsync($"{Ip}/Key/{key.Email}", content)).IsSuccessStatusCode;
    }

    /// <summary>
    /// Updates the message at a given email on the server
    /// </summary>
    /// <param name="message">The new Message object to update</param>
    /// <returns>A task resolving to whether the put succeeded or not</returns>
    public async Task<bool> PutMessage(Message message)
    {
        var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
        return (await Client.PutAsync($"{Ip}/Message/{message.Email}", content)).IsSuccessStatusCode;
    }
}