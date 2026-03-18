using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class AIReportViewModel : ViewModelBase
{
    private const string PROMPT = """
    You are an AI cybersecurity expert LLM. Below is a list of current security incidents in our monitoring system. 
    Create a brief recommendation report for the IT team on how to address these incidents, prioritize them, and prevent similar issues in the future. The answer will be displayed in a dashboard for IT staff.

    Always start with a brief summary. one paragraph with no bullet points or numbers starting with the following sentence:
    Based on the current security incidents registered in the system...
    
    After the summary paragraph provide a numbered list of recommendations based on the incident data.
    """;

    private readonly IChatGptService _chatGptService;
    private readonly IIncidentService _incidentService;

    [ObservableProperty]
    private string _response = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public AIReportViewModel(IChatGptService chatGptService, IIncidentService incidentService)
    {
        _chatGptService = chatGptService;
        _incidentService = incidentService;
    }

    [RelayCommand]
    private async Task GetRecommendationsAsync()
    {
        IsLoading = true;
        Response = string.Empty;

        var incidents = await _incidentService.ReadAllIncidentsAsync();

        var sb = new StringBuilder();
        sb.AppendLine(PROMPT);
        sb.AppendLine();

        int i = 1;
        foreach (var incident in incidents)
        {
            sb.AppendLine($"{i}. Description: {incident.Description}");
            sb.AppendLine($"   Device: {incident.Device?.Name ?? "Unknown"}");
            sb.AppendLine($"   Priority: {incident.Priority}");
            sb.AppendLine($"   Status: {incident.Status}");
            sb.AppendLine($"   Created At: {incident.CreatedAt:yyyy-MM-dd HH:mm}");
            sb.AppendLine();
            i++;
        }

        var request = new ChatGptRequest
        {
            Model = "gpt-3.5-turbo",
            Messages = new List<Message>
            {
                new Message { Role = "user", Content = sb.ToString() }
            }
        };

        var result = await _chatGptService.GenerateResponseAsync(request);
        Response = result?.Choices?[0]?.Message?.Content ?? "No response received.";
        IsLoading = false;
    }
}
