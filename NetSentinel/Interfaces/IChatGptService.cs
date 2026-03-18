using System.Threading.Tasks;

namespace NetSentinel.Interfaces;

public interface IChatGptService
{
    Task<ChatGptResponse?> GenerateResponseAsync(ChatGptRequest request);
}
