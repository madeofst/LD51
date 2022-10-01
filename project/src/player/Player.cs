using Godot;
using System;

public class Player : KinematicBody2D
{
    public Vector2 MovementDirection { get; set; } = Vector2.Zero;
    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public Vector2 FacingDirection { get; private set; }
    public float ShoeBonus { get; set; } = 1;

    public const float Acceleration = 1200;
    public const float MaxSpeed = 8000;
    public const float Friction = 4000;
    public Vector2 StartGlobalPosition = new Vector2(168, 88);

    private AnimationPlayer AnimationPlayer;
    private WorldGrid WorldGrid;
    private WorldTimer WorldTimer;


    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        WorldGrid = GetNode<WorldGrid>("../WorldGrid");
        WorldTimer = GetNode<WorldTimer>("../Camera2D/CanvasLayer/WorldTimer");
    }

    public override void _Process(float delta)
    {
        Vector2 MovementDirection;
        MovementDirection.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        MovementDirection.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        //GD.Print(MovementDirection);
        MovementDirection = MovementDirection.Normalized();
        if (MovementDirection != Vector2.Zero) FacingDirection = MovementDirection;
        this.MovementDirection = MovementDirection;
    }

    public void Teleport(Vector2 Location)
    {
        Position = Location;
    }

    public override void _PhysicsProcess(float delta)
    {
        UpdatePosition(delta);
        //GD.Print(FacingDirection);
        WorldGrid.CheckForPickups(GlobalPosition);

        if (WorldGrid.CheckForTeleports(GlobalPosition))
        if (WorldGrid.CheckForActiveTeleports(GlobalPosition)) Velocity = Vector2.Zero;
        WorldGrid.ActivateAllTeleports(GlobalPosition);
    }

    public void BackToStart()
    {
        Position = StartGlobalPosition;
        WorldTimer.Start();
    }

    private void UpdatePosition(float delta)
    {
        //GD.Print(MovementDirection);
        if (MovementDirection != Vector2.Zero)
        {
            Velocity += MovementDirection * (Acceleration * ShoeBonus) * delta;
            Velocity = Velocity.Clamped((MaxSpeed * ShoeBonus) * delta);
        }
        else
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * delta);
        }

        GD.Print($"{MovementDirection} {Velocity}");

        MoveAndCollide(Velocity * delta);
    }

    public void Idle(string AnimationName)
    {
        AnimationPlayer.Play("Idle");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_smash"))
        {
            AnimationPlayer.Play("Smash");
            WorldGrid.SmashObjects(GlobalPosition, FacingDirection);
            //FIXME: Switch lever
        }
    }
}
