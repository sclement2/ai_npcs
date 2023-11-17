using AI_NPCs;
using Godot;
using System;
using System.Threading.Tasks;

public partial class NPCChatDialogueNode : Node
{
    private OpenAIClient openAIClient;
    private NPCChatDialogue npcChatDialogue;
    private readonly SecretManager secretManager;
    private String _ChatGPTAIKey;
    private bool isChatting = false;

    // Get the NPC's name.
    [Export] string npcName = "Villager";

    // Get the player's dialogue.
    [Export] string playerDialogue = "Explain the Godot engine in 20 words";


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the API Key
        _ChatGPTAIKey = GetSecret();

        // Create a new OpenAI client.
        openAIClient = new OpenAIClient(_ChatGPTAIKey);

        // Create a new NPCChatDialogue instance.
        npcChatDialogue = new NPCChatDialogue(openAIClient);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!isChatting)
        {
            _ = Chat(npcName, playerDialogue);

        }
    }

    public NPCChatDialogueNode()
    {
        this.secretManager = new SecretManager();
    }

    private string GetSecret()
    {
        string apiSecret = secretManager.GetSecretValue("CHATGPT_GODOT_NPCAI_KEY");
        return apiSecret;
    }

    public async Task<string> GenerateChatResponse(string npcName, string playerDialogue)
    {
        // Call the GenerateChatResponse() method on the NPCChatDialogue instance.
        var chatResponse = await npcChatDialogue.GenerateChatResponse(npcName, playerDialogue);

        // Return the chat response.
        return chatResponse;
    }

    public async Task Chat(string npcName, string playerDialogue)
    {
        isChatting = true;
        var chatResponse = await GenerateChatResponse(npcName, playerDialogue);
        DisplayChatResponse(chatResponse);
    }

    private void DisplayChatResponse(string chatResponse)
    {
        GD.Print(chatResponse);
        isChatting = false;
    }
}
