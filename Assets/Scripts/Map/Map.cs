using System;   
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using PriorityQueue;
using System.IO;
using System.Linq;

class Map : IMap
{
    public Color default_tile_color = Color.white; 
    public Color path_tile_color = Color.green;
    public Color action_tile_color = Color.magenta; 
    public Color movement_tile_color = Color.blue; 
    public Color path_start_color = Color.green; 
    public Color path_goal_color = Color.green;

    private List<string> last_path_;
    public List<string> last_movement_; 

    void OnGUI()
    {
        int width = 100; 

        // Make a background box
        GUI.Box(new Rect(10,10,120,200), "Map Debug");

        if(GUI.Button(new Rect(20,40, width, 20), "GetPath"))
        {
            if (last_path_ != null)
            {
                CleanUpPath(last_path_); 
            }
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

            int random_start = UnityEngine.Random.Range(0, tiles.Length);
            int random_goal = UnityEngine.Random.Range(0, tiles.Length);

            List<string> res = GetPath(tiles[random_start].name, tiles[random_goal].name);
            ShowPath(res); 
        }

        if(GUI.Button(new Rect(20,70, width, 20), "CleanPath"))
        {
            if (last_path_ != null)
            {
                CleanUpPath(last_path_); 
            }
        }

        if(GUI.Button(new Rect(20,100, width, 20), "ShowCharSpd"))
        {
            List<string> available_goals = GetCharacterMovement("Saera");
            last_movement_ = available_goals;
            MarkTiles(available_goals, "movement");
        }
        
        if(GUI.Button(new Rect(20,130, width, 20), "CLeanCharSpd"))
        {
            MarkTiles(last_movement_, "default");
        }
    }

    public List<string> ShowCharacterSpeed(string character_name)
    {   
        List<string> available_goals = GetCharacterMovement(character_name);
        last_movement_ = available_goals;
        MarkTiles(available_goals, "movement");
        return available_goals;
    }

    public void ShowCurrentCharacterSpeed()
    {
        MarkTiles(last_movement_, "movement");
    }

    public void CleanCharacterSpeed()
    {
        if (last_movement_ != null)
        {
            MarkTiles(last_movement_, "default");
            last_movement_ = null;
        }
    }

    private Dictionary<string, string> placement_map = new Dictionary<string, string>();

    private double Heuristic(Node a, Node b)
    {
        Vector3 difference = a.game_object.transform.position - b.game_object.transform.position;
        return difference.magnitude;
    }

    public override List<string> GetPath(string start_tile_name, string goal_tile_name)
    {
        string start_node_name = start_tile_name;
        string goal_node_name = goal_tile_name;
        Node goal_node = graph[goal_node_name];

        PriorityQueue<string> frontier = new PriorityQueue<string>();
        frontier.Enqueue(0.0, start_node_name);
        OrderedDictionary came_from = new OrderedDictionary(); 
        came_from[start_node_name] = null;
        OrderedDictionary cost_so_far = new OrderedDictionary(); 
        cost_so_far[start_node_name] = 0.0;
        while (frontier.Count != 0 )
        {
            string current_node_name = frontier.Dequeue();
            if (String.Equals(current_node_name, goal_node_name))
            {
                break;
            }

            List<Node> neighbors = graph[current_node_name].neighbors;
            List<double> edge_weights = graph[current_node_name].edge_weights;
            for(int i = 0; i < neighbors.Count; i++)
            {
                Node next_node = neighbors[i];
                double edge_weight = edge_weights[i];
                double new_cost = (double)cost_so_far[current_node_name] + edge_weight;
                if (next_node.occupant != null)
                {
                    continue;
                }
                if (!cost_so_far.Contains(next_node.game_object.name)
                    || new_cost < (double)cost_so_far[next_node.game_object.name])
                {
                    cost_so_far[next_node.game_object.name] = new_cost; 
                    double priority = new_cost + Heuristic(next_node, goal_node); 
                    frontier.Enqueue(priority, next_node.game_object.name);
                    came_from[next_node.game_object.name] = current_node_name; 
                }
            }
        }

        List<string> path = new List<string>();
        string current = goal_node_name; 
        while (!String.Equals(current, start_node_name))
        {
            path.Add(current);
            current = came_from[current].ToString();
        }
        path.Add(start_node_name);
        path.Reverse();

        last_path_ = path; 
        return path;
    }

    public override void GetPathPoints(string start_tile_name, string goal_tile_name)
    {
    }

    public List<Vector3> TilesToPositions(List<string> tiles)
    {
        List<Vector3> tile_points = new List<Vector3>(); 
        foreach(string tile_name in tiles)
        {
            Vector3 tile_point = graph[tile_name].game_object.transform.position;
            tile_points.Add(tile_point);
        }
        return tile_points;
    }

    public override void GetMovementPathCost(List<string> path)
    {
    }


    public override void GetTilesInRange(string origin_tile_name, int range)
    {
    }

    public List<string> GetTilesInCost(string origin_tile_name, double cost)
    {
        string start_node_name = origin_tile_name;
        double cutoff_cost = cost; 
        PriorityQueue<string> frontier = new PriorityQueue<string>();
        frontier.Enqueue(0.0, start_node_name);
        OrderedDictionary came_from = new OrderedDictionary(); 
        came_from[start_node_name] = null;
        OrderedDictionary cost_so_far = new OrderedDictionary(); 
        cost_so_far[start_node_name] = 0.0;
        while (frontier.Count != 0 )
        {
            string current_node_name = frontier.Dequeue();

            List<Node> neighbors = graph[current_node_name].neighbors;
            List<double> edge_weights = graph[current_node_name].edge_weights;
            for(int i = 0; i < neighbors.Count; i++)
            {
                Node next_node = neighbors[i];
                double edge_weight = edge_weights[i];
                double new_cost = (double)cost_so_far[current_node_name] + edge_weight;

                if(new_cost > cutoff_cost)
                {
                    continue;
                }
                if (next_node.occupant != null)
                {
                    continue;
                }
                if (!cost_so_far.Contains(next_node.game_object.name)
                    || new_cost < (double)cost_so_far[next_node.game_object.name])
                {
                    cost_so_far[next_node.game_object.name] = new_cost; 
                    double priority = new_cost; 
                    frontier.Enqueue(priority, next_node.game_object.name);
                    came_from[next_node.game_object.name] = current_node_name; 
                }
            }
        }

        List<string> available_goals = new List<string>();
        foreach(DictionaryEntry node_cost in cost_so_far)
        {
            if((double)node_cost.Value <= cutoff_cost)
            {
                available_goals.Add(node_cost.Key.ToString());
            }
        }

        return available_goals;
    }

    public override List<string> GetCharacterMovement(string character_name)
    {
        Character character = GameObject.Find(character_name).GetComponent<Character>();
        double char_speed = character.GetSpeed(); 
        string character_current_tile = GetCharacterLocation(character_name);
        List<string> available_goals = GetTilesInCost(character_current_tile, char_speed);
        return available_goals; 
    }

    public override void GetCharacterMovement(string character, double speed)
    {
    }
    
    public override void GetCharacterAttackRange(string character)
    {
    }

    public override string GetCharacterLocation(string character_name)
    {
        return placement_map[character_name];
    }

    public void UpdatePlacementMap()
    {
        foreach( KeyValuePair<string, Node> kvp in graph )
        {
            if (kvp.Value.occupant != null) 
            {
                placement_map[kvp.Value.occupant.name] = kvp.Key;  
            }
        }
    }

    public override void GetAllCharacters()
    {
    }

    public List<string> GetAllCharacterNames()
    {
        List<string> character_names = new List<string>(); 
        foreach(KeyValuePair<string, string> kvp in placement_map)
        {
            character_names.Add(kvp.Key);
        }
        return character_names;
    }
    
    public override void GetCharacterFromTeam()
    {
    }

    public override void MoveCharacterToTile(string character_name, string new_tile_name)
    {
        string current_tile_name = placement_map[character_name];
        Node new_tile = graph[new_tile_name];
        if(new_tile.occupant == null)
        {
            new_tile.occupant = graph[current_tile_name].occupant;
            graph[current_tile_name].occupant = null;
            placement_map[character_name] = new_tile_name;
        }
        else
        {
            Debug.Log("New tile is occupied");
        }
    }
    
    public override void MarkTile(string tile_name, string mark)
    {
        Color chosen_color; 
        switch(mark) 
        {
            case "start": 
                chosen_color = path_start_color;
                break;
            case "goal": 
                chosen_color = path_goal_color;
                break; 
            case "path":
                chosen_color = path_tile_color;  
                break;
            case "action":
                chosen_color = action_tile_color;  
                break;
            case "movement":
                chosen_color = movement_tile_color;  
                break;
            default:
                chosen_color = default_tile_color;
                break;
        }
        GameObject.Find(tile_name).GetComponent<Renderer>().material.color = chosen_color; 
    }

    public override void MarkTiles(List<string> tiles, string mark)
    {
        foreach(string tile in tiles)
        {
            MarkTile(tile, mark);
        }
    }

    public override void ShowPath(List<string> path)
    {
        MarkTiles(path, "path");
        MarkTile(path[0], "start");
        MarkTile(path[path.Count-1], "goal");
    }

    public void CleanUpPath(List<string> path)
    {
        MarkTiles(path, "");
    }

    public void CleanUpLastPath()
    {
        if (last_path_ != null)
        {
            MarkTiles(last_path_, "");
        }
    }

    protected override string GetTileName(){
        return "tile_name";
    }

    public bool IsTileOccupied(string tile_name)
    {
        return graph[tile_name].occupant != null;
    }
    


}
