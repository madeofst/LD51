using Godot;
using System;

public class IntroScreen : Node2D
{
    public Game Game;
    public RichTextLabel Text1;
    public RichTextLabel Text2;
    public float time2 = 2.5f;
    public float time = 1.5f;

    public override void _Ready()
    {
        Game = ResourceLoader.Load<PackedScene>("res://src/game/Game.tscn").Instance<Game>();
        Text1 = GetNode<RichTextLabel>("CanvasLayer/Text1");
        Text2 = GetNode<RichTextLabel>("CanvasLayer/Text2");
        Text1.PercentVisible = 0;
    }


    public override void _Process(float delta)
    {
        Text1.PercentVisible += delta / 12;

        if (Text1.PercentVisible >= 1) time -= delta;

        if (time <= 0)
        {
            Text1.Visible = false;
            Text2.Visible = true;
            time2 -= delta;
        }

        if (time2 <= 0)
        {
            GetTree().Root.AddChild(Game);
            QueueFree();
        }

    }
}
