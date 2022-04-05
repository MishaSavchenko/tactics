using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMap : MonoBehaviour
{
    FieldConstructor map; 
    // Start is called before the first frame update
    public string goal_tile; 

    void Start()
    {
        map = GetComponent<FieldConstructor>(); 
    }

    public List<string> available_goals = new List<string>();
    public List<string> target_goals = new List<string>();


    public void ShowCharacterSpeed(string character_name)
    { 
        string tile_name = map.GetCharacterPlacement(character_name);
        Character character = GameObject.Find(character_name).GetComponent<Character>(); 
        double current_speed = character.GetSpeed(); 
        
        GameObject start_tile = GameObject.Find(tile_name);
        start_tile.GetComponent<Renderer>().material.color = Color.yellow; 
        
        (available_goals, target_goals) = map.get_available_goals_and_targets(start_tile.name, 
                                                                                current_speed, 
                                                                                5.0);
        
        foreach(string available_goal_name in available_goals)
        {
            GameObject available_goal_tile = GameObject.Find(available_goal_name);
            available_goal_tile.GetComponent<Renderer>().material.color = Color.blue;
        }
        foreach(string target_goal_name in target_goals)
        {
            GameObject target_goal_tile = GameObject.Find(target_goal_name);
            target_goal_tile.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void MoveCharacterToGoal(string character_name, string goal_tile_name)
    {
        if(!map.graph.ContainsKey(goal_tile_name) || !available_goals.Contains(goal_tile_name))
        {
            return;
        } 

        string start_tile_name = map.GetCharacterPlacement(character_name);

        List<string> path = map.GetPath(start_tile_name, goal_tile_name);

        List<Vector3> tile_path = new List<Vector3>();
        Character character = GameObject.Find(character_name).GetComponent<Character>(); 
        foreach(string path_tile_name in path )
        {
            
            tile_path.Add(GameObject.Find(path_tile_name).transform.position);
        }

        GameObject agent = map.graph[start_tile_name].occupant;
        agent.GetComponent<AgentHandler>().GeneratePathTrajectory(tile_path);


        
        map.placements[character_name] = goal_tile_name;
        map.graph[goal_tile_name].occupant = agent;
        map.graph[start_tile_name].occupant = null;
    
        CleanUpShownTiles();
        ShowCharacterSpeed(character_name);

    }

    public void CleanUpShownTiles()
    {
        // Clean up goal tiles
        foreach(string available_goal_name in available_goals)
        {
            GameObject available_goal_tile = GameObject.Find(available_goal_name);
            available_goal_tile.GetComponent<Renderer>().material.color = Color.white;
        }
        available_goals.Clear();

        foreach(string target_goal_name in target_goals)
        {
            GameObject target_goal_tile = GameObject.Find(target_goal_name);
            target_goal_tile.GetComponent<Renderer>().material.color = Color.white;
        }
        target_goals.Clear();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            CastRay();
        }       
    }
    public string current_character; 
    string selected_tile = "";
    void CastRay()
    {
        // RaycastHit hit;
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // if(Physics.Raycast(ray, out hit ))
        // {
        //     string tile_name = hit.collider.gameObject.name; 
        //     this.goal_tile = tile_name; 
        //     Debug.Log("Ray casted on tile : " + tile_name);
        // }
        // RaycastHit hit;
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // if(Physics.Raycast(ray, out hit ))
        // {
        //     if (selected_tile.Equals(hit.collider.gameObject.name))
        //     {
        //         if(map.graph.ContainsKey(selected_tile) && 
        //            available_goals.Contains(selected_tile))
        //         {
        //             this.MoveCharacterToGoal(current_character, selected_tile);
        //         }
        //         selected_tile = "";
        //     }
        //     else
        //     {
        //         selected_tile = hit.collider.gameObject.name; 
        //     }
        //     // {

        //     // }
        // }
    }


}
