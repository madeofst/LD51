using Godot;
using System;

public class WorldTimer : Timer
{
    public RichTextLabel RichTextLabel;
    public Player Player;
    
    public override void _Ready()
    {
        Player = GetNode<Player>("../../../Player");
        RichTextLabel = GetNode<RichTextLabel>("RichTextLabel");
    }

    public override void _Process(float delta)
    {
        int integer = (int)Mathf.Round(TimeLeft * 100);
        decimal rounded = ((decimal)integer / 100);
/*         GD.Print (rounded);
        float stepped = Math.Stepify(rounded, 0.01f);
        GD.Print (stepped); */
        string TimeString = rounded.ToString("0.00");
        //string TimeString = .ToString();
        RichTextLabel.BbcodeText = $"[center]{TimeString}[/center]";
    }

    public void TimeUp()
    {
        Player.BackToStart();
    }
}
