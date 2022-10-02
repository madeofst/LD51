using Godot;
using System;

public class Title : TextureRect
{

    public Splash Splash;
    public IntroScreen IntroScreen;

    public override void _Ready()
    {
        IntroScreen = ResourceLoader.Load<PackedScene>("res://src/menus/IntroScreen.tscn").Instance<IntroScreen>();
        Splash = GetNode<Splash>("/root/SplashScreen/Splash");
        Splash.Deactivate();
    }

    public override void _Input(InputEvent @event)
    {

        if (@event.IsActionPressed("ui_select") || @event.IsActionPressed("ui_accept"))
        {
            if (GetNode<Game>("/root/Game") == null)
            {
                GetTree().Root.AddChild(IntroScreen);
                GetParent().QueueFree();
            }
        }
    }
}
