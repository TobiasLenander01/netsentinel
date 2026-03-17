using System.Collections.Generic;

namespace NetSentinel;

public class ChatGptRequest
{
    public required string Model { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
}
