namespace NetSentinel;

public class Choice
{
    public required string FinishReason { get; set; }
    public int Index { get; set; }
    public required Message Message { get; set; }
    public object? Logprobs { get; set; }
}
