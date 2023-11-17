using AI_NPCs;
using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class GameManager : Node
{
    private readonly SecretManager secretManager;
    private String _ChatGPTAIKey;

    // Chat model info
    private readonly String _apiURL = "https://api.openai.com/v1/chat/completions";
    [Export] private float _temperature = 0.5f;
    [Export] private int _maxTokens = 1024;
    [Export] private String _model = "gpt-3.5-turbo";
    private String[] _headers;

    // Get the NPC's name.
    [Export] string npcName = "Villager";

    // Get the player's dialogue.
    [Export] string _playerDialogue = "Explain the Godot engine in 20 words";

    // Dialogue rules
    [Export]
    string dialogueRules = "You are in a fantasy RPG engaging me in conversation. " +
                "Reply to my questions and never break character. Don't mention that you are an AI or that this is a game. " +
                "Only respond with your dialogue text and nothing else, but you can imply actions with []. " +
                "For example: [laughs].";

    //private List<OpenAIDialogueMessage> _conversation = new();
    private List<Dictionary<string, string>> _conversation = new();
    private HttpRequest _request;

    public GameManager()
    {
        this.secretManager = new SecretManager();
    }

    private string GetSecret()
    {
        string apiSecret = secretManager.GetSecretValue("CHATGPT_GODOT_NPCAI_KEY");
        return apiSecret;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _ChatGPTAIKey = GetSecret();
        _headers = new String[] { "Content-type: application/json", "Authorization: Bearer " + _ChatGPTAIKey };
        //GD.Print("Secret: " + _ChatGPTAIKey + "\n");

        //Create an HTTP request node and connect its completion signal.
        _request = new HttpRequest();
        AddChild(_request);
        _request.Connect("request_completed", new Callable(this, "OnRequestCompleted"));

        DialogueRequest(_playerDialogue);
    }

    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        //GD.Print("Inside 'OnRequestCompleted' method");

        if (responseCode == 200)
        {
            GD.Print("Got response from OpenAI \n");
            // Handle the response body or any other logic
            string responseBody = System.Text.Encoding.UTF8.GetString(body);
            JsonDocument jsonDoc = JsonDocument.Parse(responseBody);
            JsonElement root = jsonDoc.RootElement;

            string message = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            GD.Print(message);

        }
        else
        {
            GD.Print("Error: " + responseCode);
        }
    }

    public void DialogueRequest(string playerDialogue)
    {
        string prompt = playerDialogue;

        if (_conversation.Count == 0)
        {
            // CurrentNPC.physicalDescription 
            // CurrentNPC.personality
            // CurrentNPC.locationDescription
            // CurrentNPC.SecretKnowledge
            string physicalDescription = "An old wizard with a purple robe and long white beard.";
            string personality = "Wise, smart, mysterious.";
            string locationDescription = "Walking along a path in the village.";
            string secretKnowledge = "You know the password to gain access to the Knight's house. It is the word Please.";

            string headerPrompt = "Act as a " + physicalDescription + " in a fantasy RPG. ";

            headerPrompt += "As a character, you are " + personality + ". ";
            headerPrompt += "Your location is " + locationDescription + ". ";
            headerPrompt += "You have secret knowledge that you will not speak about unless asked by me: " + secretKnowledge + ". ";

            prompt = dialogueRules + "\n" + headerPrompt + "\nWhat is your first line of dialogue?";
        }


        GD.Print("Prompt sent to OpenAI: " + prompt + "\n");

        // add a new message to the array
        //_conversation.Add(new OpenAIDialogueMessage { Role = "user", Content = prompt });
        _conversation.Add(new Dictionary<string, string>
            {
                { "role", "user" },
                { "content", prompt }
            });

        // create the request body
        Dictionary<string, object> postData = new Dictionary<string, object>
        {
            { "messages", _conversation },
            { "temperature", _temperature },
            { "max_tokens", _maxTokens },
            { "model", _model },
        };

        string postDataJson = JsonConvert.SerializeObject(postData);

        //GD.Print($"postDataJson: {postDataJson}");

        Error sendRequest = _request.Request(_apiURL, _headers, HttpClient.Method.Post, postDataJson);

        // if there was a problem, make it known
        if (sendRequest != Error.Ok)
        {
            GD.Print("There was an error!");
        }
    }
}