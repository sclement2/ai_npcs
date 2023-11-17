using Godot;

public partial class NPC : CharacterBody2D
{
    [Export] Texture2D icon;
    [Export] string physicalDescription;
    [Export] string locationDescription;
    [Export] string personality;
    [Export] string secretKnowledge;

}
