using System.Text.Json;

namespace CodeChallenge.Services;
public interface ITokenService
{
    Task<string?> GetTokenAsync();
}

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _baseUrl;
    private readonly string _email;

    public TokenService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _baseUrl = $"{configuration["CodeChallengeApi:BaseUrl"]}";
        _email = $"{configuration["CodeChallengeApi:Email"]}";
    }

    public async Task<string?> GetTokenAsync()
    {
        var client = _clientFactory.CreateClient();
        var response = await client.PostAsync(_baseUrl + "/login",
            new StringContent(JsonSerializer.Serialize(new { email = _email }),
                System.Text.Encoding.UTF8,
                "application/json"));
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content);
        return json.GetProperty("token").GetString();
    }
}