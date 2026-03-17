namespace NetSentinel;

public class Message
{
    public required string Role { get; set; } // "user" or "assistant"
    public required string Content { get; set; }
}
