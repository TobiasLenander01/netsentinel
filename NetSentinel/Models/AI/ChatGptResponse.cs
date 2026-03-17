using System.Collections.Generic;

namespace NetSentinel;

public class ChatGptResponse
{
    public required List<Choice> Choices { get; set; }
    public long Created { get; set; }
    public required string Id { get; set; }
    public required string Model { get; set; }
    public required string Object { get; set; }
    public required Usage Usage { get; set; }
}
