using Godot;
using System;

public class Player : KinematicBody2D
{
    public Vector2 MovementDirection { get; set; } = Vector2.Zero;
    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public Vector2 FacingDirection { get; private set; }

    public const float Acceleration = 10000;
    public const float MaxSpeed = 8000;
    public const float Friction = 1200;

    private AnimationPlayer AnimationPlayer;
    private WorldGrid WorldGrid;

    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        WorldGrid = GetNode<WorldGrid>("../WorldGrid");
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

    public override void _PhysicsProcess(float delta)
    {
        UpdatePosition(delta);
        //GD.Print(FacingDirection);
    }

    private void UpdatePosition(float delta)
    {
        //GD.Print(MovementDirection);
        Velocity += MovementDirection * Acceleration * delta;
        Velocity = Velocity.Clamped(MaxSpeed * delta);
        Velocity = Velocity.MoveToward(Vector2.Zero, Friction * delta);

        //GD.Print($"{MovementDirection} {Velocity}");
        
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
        }
    }
}
