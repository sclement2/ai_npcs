using Newtonsoft.Json;

namespace AI_NPCs
{
    internal class OpenAIRequest
    {
        public string prompt;
        public string model;
        public double temperature;
        public int maxTokens;

        public OpenAIRequest(string prompt, string model = "text-davinci", double temperature = 0.7, int maxTokens = 500)
        {
            this.prompt = prompt;
            this.model = model;
            this.temperature = temperature;
            this.maxTokens = maxTokens;
        }

        public string ToJson()
        {
            //return JsonUtility.ToJson(this);
            return JsonConvert.SerializeObject(this);
        }
    }
}
