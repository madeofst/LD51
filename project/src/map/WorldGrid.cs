using Godot;
using System;

public class WorldGrid : TileMap
{
    public Player Player;

    public override void _Ready()
    {
        Player = GetNode<Player>("../Player");
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

    internal void CheckForPickups(Vector2 PlayerGlobalPosition)
    {
        Vector2 TargetTile = WorldToMap(PlayerGlobalPosition);
        if (GetCellv(TargetTile) == 4) //Shoe
        {
            SetCellv(TargetTile, -1);
            Player.ShoeBonus = Player.MaxSpeed * 0.5f;   
        }
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
} 
