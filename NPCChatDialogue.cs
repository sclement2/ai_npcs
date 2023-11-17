using Godot;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_NPCs
{
    internal class NPCChatDialogue
    {
        private OpenAIClient openAIClient;
        private Dictionary<string, Dictionary<string, string>> npcConversationHistory;

        public NPCChatDialogue(OpenAIClient openAIClient)
        {
            this.openAIClient = openAIClient;
            this.npcConversationHistory = new Dictionary<string, Dictionary<string, string>>();
        }

        public async Task<string> GenerateChatResponse(string npcName, string playerDialogue)
        {
            GD.Print("Inside the NPCChatDialog");

            // Get the NPC's conversation history.
            var conversationHistory = npcConversationHistory[npcName];

            // Add the current conversation turn to the history.
            conversationHistory["user"] = playerDialogue;

            var request = new OpenAIRequest(string.Join("\n", conversationHistory) + "\n\nWhat can I help you with today?");

            // Generate a new chat response.
            var response = await openAIClient.SendRequest(request);
            GD.Print(response.ToString());

            var chatResponse = JsonConvert.DeserializeObject<ChatGPTResponse>(response);

            // Update the NPC's conversation history.
            conversationHistory["npc"] = chatResponse.choices[0].content;

            GD.Print(chatResponse.choices[0].content);
            return chatResponse.choices[0].content;
        }
    }
}
