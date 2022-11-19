// Author: John Kudela

namespace SecureMessaging.Model;

/*
 * Used for sending a message to the server
 */
public class Message
{
    public Message(string email, string content)
    {
        this.Email = email;
        this.Content = content;
    }

    public string Email { get; set; }
    public string Content { get; set; }
}