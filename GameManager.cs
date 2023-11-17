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

    private readonly String _apiURL = "https://api.openai.com/v1/chat/completions";
    private float _temperature = 0.5f;
    private int _maxTokens = 1024;
    private String[] _headers;
    private String _model = "gpt-3.5-turbo";

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
        GD.Print("Secret: " + _ChatGPTAIKey);

        //Create an HTTP request node and connect its completion signal.
        _request = new HttpRequest();
        AddChild(_request);
        _request.Connect("request_completed", new Callable(this, "OnRequestCompleted"));

        DialogueRequest("Explain the Godot engine in 20 words");
    }

    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        GD.Print("Inside 'OnRequestCompleted' method");

        if (responseCode == 200)
        {
            GD.Print("Got response");
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
        GD.Print($"{playerDialogue}");

        _conversation.Add(new Dictionary<string, string>
            {
                { "role", "user" },
                { "content", playerDialogue }
            });

        GD.Print($"Content: {_conversation[0]["content"]}");

        Dictionary<string, object> postData = new Dictionary<string, object>
        {
            { "messages", _conversation },
            { "temperature", _temperature },
            { "max_tokens", _maxTokens },
            { "model", _model },
        };

        string postDataJson = JsonConvert.SerializeObject(postData);


        GD.Print($"postDataJson: {postDataJson}");

        _request.Request(_apiURL, _headers, Godot.HttpClient.Method.Post, postDataJson);

    }
}