using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using ShadUI;

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
    private readonly ToastManager _toastManager;

    [ObservableProperty]
    private string _response = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public AIReportViewModel(IChatGptService chatGptService, IIncidentService incidentService, ToastManager toastManager)
    {
        _chatGptService = chatGptService;
        _incidentService = incidentService;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private async Task GetRecommendationsAsync()
    {
        try
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
        }
        catch (InvalidOperationException)
        {
            _toastManager
                .CreateToast("AI service not configured")
                .DismissOnClick()
                .ShowError();
            Response = "Unable to generate AI recommendations because the AI service is not configured.";
        }
        catch (Exception)
        {
            _toastManager
                .CreateToast("Failed to generate recommendations")
                .WithContent("Please try again in a moment.")
                .DismissOnClick()
                .ShowError();
            Response = "An error occurred while generating the AI report.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
