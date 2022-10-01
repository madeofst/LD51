using Godot;
using System;

public class WorldTimer : Timer
{
    public RichTextLabel RichTextLabel;
    public Player Player;
    
    public override void _Ready()
    {
        Player = GetNode<Player>("../Player");
        RichTextLabel = GetNode<RichTextLabel>("RichTextLabel");
    }

    public override void _Process(float delta)
    {
        RichTextLabel.Text = TimeLeft.ToString();
    }

    public void TimeUp()
    {
        Player.BackToStart();
    }
}
