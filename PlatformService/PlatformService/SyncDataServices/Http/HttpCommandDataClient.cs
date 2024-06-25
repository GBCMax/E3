using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient p_httpClient;
        private readonly IConfiguration p_config;
        public HttpCommandDataClient(HttpClient _httpClient, IConfiguration _config)
        {
            p_httpClient = _httpClient;
            p_config = _config;
        }
        public async Task SendPlatformToCommand(PlatformReadDto _platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(_platform),
                Encoding.UTF8,
                "application/json"
            );

            var response = await p_httpClient.PostAsync($"{p_config["CommandService"]}", httpContent);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
            }
        }
    }
}