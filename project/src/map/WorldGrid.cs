using Godot;
using System;
using System.Collections.Generic;

public class WorldGrid : TileMap
{
    public const int iBoxSet = 3;
    public const int iGateSet = 12;
    public const int iWallSet = 2;
    public const int iLeverSet = 7;
    public const int iShoesSet = 4;
    public const int iTeleportSet = 11;
    public const int iDoorSet = 9;

    public Player Player;
    public VictoryScreen VictoryScreen;
    private AudioStreamPlayer GateSound;
    private AudioStreamPlayer ShoeSound;
    private AudioStreamPlayer BoxSound;
    private AudioStreamPlayer TeleportSound;
    private WorldTimer WorldTimer;

    private TileMap FloorFeatures;
    private TileMap Floor;

    Dictionary<Vector2, Vector2> TeleportReferences;
    Dictionary<Vector2, Vector2> LeverReferences;

    public override void _Ready()
    {
        Player = GetNode<Player>("../Player");
        VictoryScreen = ResourceLoader.Load<PackedScene>("res://src/menus/VictoryScreen.tscn").Instance<VictoryScreen>();
        GateSound = GetNode<AudioStreamPlayer>("../Gate");
        ShoeSound = GetNode<AudioStreamPlayer>("../Shoes");
        BoxSound = GetNode<AudioStreamPlayer>("../Box");
        TeleportSound = GetNode<AudioStreamPlayer>("../Teleport");
        WorldTimer = GetNode<WorldTimer>("../Camera2D/CanvasLayer/WorldTimer");

        Floor = GetNode<TileMap>("../Floor");
        FloorFeatures = GetNode<TileMap>("../FloorFeatures");

        TeleportReferences = new Dictionary<Vector2, Vector2>()
        {
            //Forwards
            { new Vector2(32, 5), new Vector2(9, 12) },
            { new Vector2(29, -1), new Vector2(-4, 15) },

            //Backwards
            { new Vector2(9, 12), new Vector2(32, 5) },
            { new Vector2(-4, 15), new Vector2(29, -1) }
        };

        LeverReferences = new Dictionary<Vector2, Vector2>()
        {
            //Forwards
            { new Vector2(16, 8), new Vector2(7, 7) },
            { new Vector2(29, -7), new Vector2(23, 0) },
            { new Vector2(23, 14), new Vector2(15, 15) },
            { new Vector2(44, -4), new Vector2(35, 5) },
        };
    }

    public void SmashObjects(Vector2 PlayerGlobalPosition, Vector2 FacingDirection)
    {
        Vector2 TargetTile = WorldToMap(PlayerGlobalPosition);
        if (GetCellv(TargetTile) == iLeverSet) //Lever
        {
            OpenGateFromLever(TargetTile);
            SetCellv(TargetTile, iLeverSet, false, false, false, new Vector2(1, 0));
        }

        TargetTile = TargetTile + FacingDirection;
        if (GetCellv(TargetTile) == iBoxSet && GetCellAutotileCoord((int)TargetTile.x, (int)TargetTile.y) == Vector2.Zero) //Box
        {
            BoxSound.Play(0.05f);
            SetCellv(TargetTile, iBoxSet, false, false, false, new Vector2(1, 0));
        }
        else if (GetCellv(TargetTile) == iDoorSet)
        {
            VictoryScreen.Test();
            GetTree().Root.AddChild(VictoryScreen);
            GetParent().QueueFree();
        }
        
        //GD.Print(WorldToMap(PlayerGlobalPosition));
    }

    private void OpenGateFromLever(Vector2 LeverTile)
     {   
        Vector2 TargetTile;
        LeverReferences.TryGetValue(LeverTile, out TargetTile);
        GateSound.Play();
        SetCellv(TargetTile, -1);
    }

    internal void CheckForPickups(Vector2 PlayerGlobalPosition)
    {
        Vector2 TargetTile = WorldToMap(PlayerGlobalPosition);
        if (GetCellv(TargetTile) == iShoesSet)
        {
            ShoeSound.Play();
            WorldTimer.ShoeMessage.PercentVisible = 1;
            SetCellv(TargetTile, -1);
            Player.ShoeBonus = 1.5f;   
        }
    }

    public bool CheckForActiveTeleports(Vector2 PlayerGlobalPosition)
    {
        Vector2 EnterTile = WorldToMap(PlayerGlobalPosition);
        if (GetCellv(EnterTile) == iTeleportSet && TeleportActive(EnterTile))
        {
            Vector2 ExitTile = FindTeleportExit(EnterTile);
            if (ExitTile != EnterTile)
            {
                Player.Teleporting = true;
                ToggleTeleportActive(ExitTile);
                TeleportSound.Play();
                Modulate = new Color(0.36f, 0.8f, 0.87f, 1f);
                FloorFeatures.Modulate = new Color(0.36f, 0.8f, 0.87f, 1f);
                Floor.Modulate = new Color(0.36f, 0.8f, 0.87f, 1f);
                Player.Teleport(MapToWorld(ExitTile) + new Vector2(8, 8));
                return true;
            };
        }
        return false;
    }

    public bool CheckForTeleports(Vector2 PlayerGlobalPosition)
    {
        Vector2 EnterTile = WorldToMap(PlayerGlobalPosition);   
        if (GetCellv(EnterTile) == iTeleportSet) return true;
        return false;
    }

    private bool TeleportActive(Vector2 TeleportTile)
    {
        if (GetCellAutotileCoord((int)TeleportTile.x, (int)TeleportTile.y).x == 0f) return true;
        return false;
    }

    private void ToggleTeleportActive(Vector2 TeleportTile)
    {
        float CurrentState = GetCellAutotileCoord((int)TeleportTile.x, (int)TeleportTile.y).x;
        if (CurrentState == 0) //Active
        {
            SetCellv(TeleportTile, GetCellv(TeleportTile), false, false, false, new Vector2(1, 0));
        }
        else
        {
            SetCellv(TeleportTile, GetCellv(TeleportTile), false, false, false, new Vector2(0, 0));
        }
        CurrentState = GetCellAutotileCoord((int)TeleportTile.x, (int)TeleportTile.y).x;
    }

    private Vector2 FindTeleportExit(Vector2 EnterTile)
    {
        Vector2 ExitTile = EnterTile;
        

        TeleportReferences.TryGetValue(EnterTile, out ExitTile);
        if (!TeleportActive(ExitTile)) ExitTile = EnterTile;
        return ExitTile;
    }

    public void ReopenTeleport(Vector2 PlayerGlobalPosition)
    {
        if (!Player.Teleporting)
        {
            Vector2 TilePosition = WorldToMap(PlayerGlobalPosition);
            if (!TeleportActive(TilePosition))
            {
                ToggleTeleportActive(TilePosition);
            }
        }
    }

    public void ActivateAllTeleports(Vector2 PlayerGlobalPosition)
    {
        if (!Player.Teleporting)
        {
            foreach (KeyValuePair<Vector2, Vector2> entry in TeleportReferences)
            {
                if (entry.Key != WorldToMap(PlayerGlobalPosition))
                {
                    if (!TeleportActive(entry.Key))
                    {
                        ToggleTeleportActive(entry.Key);
                    }
                }
            }
        }
    }

} 
