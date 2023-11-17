using Godot;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AI_NPCs
{
    internal class OpenAIClient
    {
        public string apiKey;

        public OpenAIClient(string api_key)
        {
            this.apiKey = api_key;
        }

        public async Task<string> SendRequest(OpenAIRequest request)
        {
            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

            var content = new StringContent(request.ToJson(), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/engines/davinci/completions", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                GD.Print(responseContent);
                return responseContent;
            }
            else
            {
                GD.PrintErr(response);
                throw new Exception("Failed to send OpenAI request: " + response.StatusCode);
            }
        }
    }
}
