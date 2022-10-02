using Godot;
using System;

public class Player : KinematicBody2D
{
    public Vector2 MovementDirection { get; set; } = Vector2.Zero;
    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public Vector2 FacingDirection { get; private set; }
    public float ShoeBonus { get; set; } = 1;
    public bool Teleporting { get; set; } = false;
    private Vector2 TeleportTarget { get; set; }
    public bool Smashing { get; private set; } = false;

    public const float Acceleration = 1500;
    public const float MaxSpeed = 100;
    public const float Friction = 4000;
    public Vector2 StartGlobalPosition = new Vector2(122, 88);

    private AnimationPlayer AnimationPlayer;
    private AnimationTree AnimationTree;
    private AnimationNodeStateMachinePlayback StateMachine;
    private WorldGrid WorldGrid;
    private TileMap Floor;
    private WorldTimer WorldTimer;
    private Sprite Sprite;
    private CollisionShape2D CollisionShape2D;


    public override void _Ready()
    {
        WorldGrid = GetNode<WorldGrid>("../WorldGrid");
        WorldTimer = GetNode<WorldTimer>("../Camera2D/CanvasLayer/WorldTimer");
        Sprite = GetNode<Sprite>("Sprite");
        CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        Floor = GetNode<TileMap>("../Floor");
        
        AnimationTree = GetNode<AnimationTree>("AnimationTree");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        StateMachine = (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback");
        AnimationTree.Active = true;
    }

    public override void _Process(float delta)
    {
        Vector2 MovementDirection = Vector2.Zero;
        if (!Teleporting)
        {
            MovementDirection.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
            MovementDirection.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
            //GD.Print(MovementDirection);
            MovementDirection = MovementDirection.Normalized();
            if (MovementDirection != Vector2.Zero) FacingDirection = MovementDirection;
        }
        this.MovementDirection = MovementDirection;
    }

    public void Teleport(Vector2 Location)
    {
        WorldTimer.Paused = true;
        Sprite.SelfModulate = new Color(1, 1, 1, 0.5f);
        CollisionShape2D.Disabled = true;
        TeleportTarget = Location;
    }

    public override void _PhysicsProcess(float delta)
    {
        UpdatePosition(delta);
        if (!Teleporting)
        {
            WorldGrid.CheckForPickups(GlobalPosition);
            if (WorldGrid.CheckForTeleports(GlobalPosition))
            if (WorldGrid.CheckForActiveTeleports(GlobalPosition)) Velocity = Vector2.Zero;
        }
        WorldGrid.ActivateAllTeleports(GlobalPosition);
    }

    public void BackToStart()
    {
        Teleporting = true;
        WorldGrid.Modulate = new Color(0.4f, 0.42f, 0.44f, 1f);
        Floor.Modulate = new Color(0.4f, 0.42f, 0.44f, 1f);
        Teleport(StartGlobalPosition);
    }

    private void UpdatePosition(float delta)
    {
        //GD.Print(MovementDirection);
        if (Teleporting)
        {
            if (Position.IsEqualApprox(TeleportTarget))
            {
                Teleporting = false;
                Sprite.SelfModulate = new Color(1, 1, 1, 1f);
                WorldTimer.Paused = false;
                if (TeleportTarget.IsEqualApprox(StartGlobalPosition))
                {
                    WorldGrid.Modulate = new Color(1f, 1f, 1f, 1f);
                    Floor.Modulate = new Color(1f, 1f, 1f, 1f);
                    WorldTimer.Start();
                }
                CollisionShape2D.Disabled = false;
            }
            else
            {
                Position = Position.MoveToward(TeleportTarget, MaxSpeed * 2 * delta);
            }
        }
        else if (MovementDirection != Vector2.Zero && !Smashing)
        {
            AnimationTree.Set("parameters/Idle/blend_position", MovementDirection);
            AnimationTree.Set("parameters/Run/blend_position", MovementDirection);
            
            StateMachine.Travel("Run");
            Velocity += MovementDirection * (Acceleration) * delta;
            Velocity = Velocity.Clamped((MaxSpeed * ShoeBonus));
        }
        else
        {
            if (!Smashing) StateMachine.Travel("Idle");
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * ShoeBonus * delta);
        }

        //GD.Print($"{MovementDirection} {Velocity}");

        if (!Teleporting) Velocity = MoveAndSlide(Velocity);
    }

    public void SmashComplete()
    {
        Smashing = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_smash"))
        {
            AnimationTree.Set("parameters/Smash/blend_position", FacingDirection);
            Smashing = true;
            StateMachine.Travel("Smash");
            WorldGrid.SmashObjects(GlobalPosition, FacingDirection);
            //FIXME: Switch lever
        }
    }
}
