using Godot;
using System;

public class Victory : TextureRect
{
    public Splash Splash;
    public TitleScreen TitleScreen;

    public override void _Ready()
    {
        Splash = GetNode<Splash>("/root/SplashScreen/Splash");
        TitleScreen = ResourceLoader.Load<PackedScene>("res://src/menus/TitleScreen.tscn").Instance<TitleScreen>();
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_select") || @event.IsActionPressed("ui_accept"))
        {
            GetTree().Root.AddChild(TitleScreen);
            Splash.Activate();
            GetParent().QueueFree();
        }

    }
}
