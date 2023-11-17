using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AI_NPCs
{
    public class ChatGPTClient
    {
        private const string apiKey = "YOUR_API_KEY_HERE";
        private const string endpoint = "https://api.openai.com/v1/engines/davinci-codex/completions";

        public async Task<string> SendChatMessage(string message)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var request = new
                {
                    messages = new[]
                    {
                        new { role = "system", content = "You are a helpful assistant." },
                        new { role = "user", content = message }
                    }
                };

                string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);

                var content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Process and return the AI response here
                return responseContent;


                //string responseJson = await client.PostAsync(endpoint, content).Result.Content.ReadAsStringAsync();
                //var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatGPTResponse>(responseJson);
                //string aiReply = responseObj.choices[0].content;

            }
            catch (Exception e)
            {
                // Handle exceptions
                Console.WriteLine($"Error: {e.Message}");
                return "An error occurred.";
            }
        }
    }

    public class ChatGPTResponse
    {
        public Message[] choices { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}

