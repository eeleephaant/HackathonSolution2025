using Microsoft.Extensions.AI;

namespace AIChatService.Services;

public interface IAiService
{
    public Task<string> GetCompletion(string prompt);
}

public class AiService(IConfiguration configuration) : IAiService
{
    private readonly string _endpoint = configuration["AI:Endpoint"] ?? string.Empty;
    private readonly string _model = configuration["AI:Model"] ?? string.Empty;

    public async Task<string> GetCompletion(string prompt)
    {
        IChatClient client =
            new OllamaChatClient(new Uri(_endpoint), _model);

        var response = await client.GetResponseAsync(prompt);

        return response.Text;
    }
}