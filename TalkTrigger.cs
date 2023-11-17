using Godot;
using System;

public partial class TalkTrigger : Area2D
{
    public override void _Ready()
    {
        GameManager = GetNode("/root/Main/GameManager") as GameManager;
        CurrentNpc = null;
    }

    public GameManager GameManager { get; set; }
    public Node CurrentNpc { get; set; }

    public void _OnBodyEntered(Node body)
    {
        if (body.IsInGroup("NPC"))
        {
            CurrentNpc = body;
        }
    }

    public void _OnBodyExited(Node body)
    {
        if (CurrentNpc == body)
        {
            CurrentNpc = null;
        }
    }

    /*
    public override void _Input(InputEvent inputEvent)
    {
        if (Input.IsKeyPressed(Key.F) && GameManager != null && GameManager.IsDialogueActive() == false)
        {
            if (CurrentNpc != null)
            {
                GameManager.EnterNewDialogue(CurrentNpc);
            }
        }
    }
    */
}
