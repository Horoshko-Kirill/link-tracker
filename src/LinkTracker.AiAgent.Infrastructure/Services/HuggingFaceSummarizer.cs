using System.Net.Http.Headers;
using System.Net.Http.Json;
using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Contracts.Dto;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Infrastructure.Services;

public class HuggingFaceSummarizer : ISummarizer
{
    private readonly AiAgentOptions _options;
    private readonly HttpClient _httpClient;

    public HuggingFaceSummarizer(IOptions<AiAgentOptions> options, HttpClient httpClient)
    {
        _options = options.Value;
        _httpClient = httpClient;
    }
    
    public async Task<string> SummarizeAsync(string text, CancellationToken cancellationToken = default)
    {
        if (text.Length <= _options.SummarizationOptions.Threshould)
        {
            return text;
        }
        
        var prompt = $"Summarize the following update in 2-3 sentences:\n\n{text}";
        
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api-inference.huggingface.co/models/{_options.SummarizationOptions.Model}");
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.SummarizationOptions.ApiKey);
        
        request.Content = JsonContent.Create(new
        {
            inputs = prompt
        });

        var response = await _httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<HuggingFaceResponse>>(cancellationToken: cancellationToken);

        return result?.FirstOrDefault()?.SummaryText ?? text;
    }
}