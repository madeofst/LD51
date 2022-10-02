using Godot;
using System;

public class Splash : TextureButton
{

    public TitleScreen TitleScreen;

    public override void _Ready()
    {
        
    }

    public override void _Input(InputEvent @event)
    {

        if (@event.IsActionPressed("ui_select") || @event.IsActionPressed("ui_accept"))
        {
            Start();
        }
    }
    
    public void Start()
    {
        if (GetNode<Game>("/root/Game") == null && GetNode<VictoryScreen>("/root/VictoryScreen") == null && GetNode<IntroScreen>("/root/IntroScreen") == null)
        {
            TitleScreen = ResourceLoader.Load<PackedScene>("res://src/menus/TitleScreen.tscn").Instance<TitleScreen>();
            GetTree().Root.AddChild(TitleScreen);
        }
    }

    public void Deactivate()
    {
        SetProcessInput(false);
    }
    
    public void Activate()
    {
        SetProcessInput(false);
    }
}
