using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class AIReportViewModel : ViewModelBase
{
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
        sb.AppendLine("You are a cybersecurity expert. Below is a list of current security incidents in our network monitoring system. Please provide step-by-step recommendations for actions to address each incident:");
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
