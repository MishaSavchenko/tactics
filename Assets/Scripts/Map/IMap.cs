using System;   
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using PriorityQueue;
using System.IO;
using System.Linq;


public abstract class IMap : MonoBehaviour
{
    public Dictionary<string, Node> graph; 
    public List<string> characters;

    // Abstract Methods
    public abstract List<string> GetPath(string start_tile_name, string goal_tile_name);
    public abstract void GetPathPoints(string start_tile_name, string goal_tile_name);
    public abstract void GetMovementPathCost(List<string> path);
    public abstract void GetTilesInRange(string origin_tile_name, int range);

    public abstract List<string> GetCharacterMovement(string character_name);
    public abstract void GetCharacterMovement(string character, double speed);
    public abstract void GetCharacterAttackRange(string character);

    public abstract string GetCharacterLocation(string character_name);
    public abstract void GetAllCharacters();
    public abstract void GetCharacterFromTeam();
    public abstract void MoveCharacterToTile(string character_name, string new_tile_name);
    
    public abstract void MarkTile(string tile_name, string mark);
    public abstract void MarkTiles(List<string> tiles, string mark);

    public abstract void ShowPath(List<string> path);

    protected abstract string GetTileName(); 
}
