using Godot;
using System;

public class WorldGrid : TileMap
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void SmashObjects(Vector2 PlayerGlobalPosition, Vector2 FacingDirection)
    {
        Vector2 TargetTile = WorldToMap(PlayerGlobalPosition) + FacingDirection;
        if (GetCellv(TargetTile) == 3 && GetCellAutotileCoord((int)TargetTile.x, (int)TargetTile.y) == Vector2.Zero) //Box
        {
            SetCellv(TargetTile, 3, false, false, false, new Vector2(1, 0));
        }
        GD.Print(WorldToMap(PlayerGlobalPosition));
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
} 
