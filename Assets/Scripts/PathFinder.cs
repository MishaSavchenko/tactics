using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{

    Transform camera_pose; 
 
    GameObject start_tile = null;
    GameObject goal_tile = null;

    List<string> path;

    private FieldConstructor field_constructor;
    // private AgentHandler agent_handler;

    private LineRenderer line;

    private AgentInterfaceManager agent_interface_manager;

    void Start()
    {
        field_constructor = GameObject.FindObjectOfType<FieldConstructor>();
        // agent_handler = GameObject.FindObjectOfType<AgentHandler>();

        GameObject lineObject = new GameObject("Line");
        line = lineObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.startColor = c1;
        line.endColor = c2;

        agent_interface_manager = GameObject.Find("AgentInterfaceManager").GetComponent<AgentInterfaceManager>();
    }

    // Creates a line renderer that follows a Sin() function
    // and animates it.
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 20;
    public double cutoff_cost = 4.0;

    private List<string> available_goals = new List<string>();
    private List<string> target_goals = new List<string>();


    void Update()
    {
        // if (Input.GetMouseButtonDown(0)) {
        //     CastRay();
        // }       
    }

    void DrawArrow(List<Vector3> path)
    {
        line.positionCount = path.Count;
        Vector3 prev_position = path[0];
        Vector3 line_offset = new Vector3(0.0f,0.15f,0.0f);
        line.SetPosition(0, prev_position + line_offset);
        for(int i=1; i < path.Count; i++)
        {   
            Vector3 new_position = path[i] + line_offset;
            line.SetPosition(i, new_position); 
        }
    }

    void CleanUpTiles(bool start=false, bool goal=false)
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

        if(start)
        {
            start_tile.GetComponent<Renderer>().material.color = Color.white;
            start_tile = null;
        }

        if(goal)
        {
            goal_tile.GetComponent<Renderer>().material.color = Color.white;
            goal_tile = null;
        }
    }

    public string active_team; 

    void CastRay() 
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit ))
        {
            String tile_name = hit.collider.gameObject.name; 
            if (start_tile && goal_tile)
            {
                // if we want to cancel the action 
                if (String.Equals(tile_name, start_tile.name))
                {   
                    CleanUpTiles(true);
                }
                // if we want to confirm the action
                else if(String.Equals(tile_name, goal_tile.name))
                {
                    start_tile.GetComponent<Renderer>().material.color = Color.green; 
                    goal_tile.GetComponent<Renderer>().material.color = Color.green;
                    path = field_constructor.GetPath(start_tile.name, goal_tile.name);

                    List<Vector3> tile_path = new List<Vector3>();
                    foreach(string path_tile_name in path )
                    {
                        // GameObject.Find(path_tile_name).GetComponent<Renderer>().material.color = Color.black;
                        tile_path.Add(GameObject.Find(path_tile_name).transform.position);
                    }

                    // agent_handler.GeneratePathTrajectory(tile_path);
                    GameObject agent = field_constructor.graph[start_tile.name].occupant;
                    agent.GetComponent<AgentHandler>().GeneratePathTrajectory(tile_path);
                    field_constructor.graph[goal_tile.name].occupant = agent;
                    field_constructor.graph[start_tile.name].occupant = null;

                    DrawArrow(tile_path);
                    CleanUpTiles(true, true);
                }
                // if we want to choose a different target
                else
                {
                    goal_tile.GetComponent<Renderer>().material.color = Color.white; 
                    goal_tile = GameObject.Find(tile_name);
                    goal_tile.GetComponent<Renderer>().material.color = Color.red; 
                }

            } 
            else if (start_tile && !goal_tile)
            {
                if(String.Equals(tile_name, start_tile.name))
                {
                    start_tile.GetComponent<Renderer>().material.color = Color.white; 
                    CleanUpTiles(true, false);

                }
                else
                {
                    if(available_goals.Contains(tile_name))
                    {
                        goal_tile = GameObject.Find(tile_name);
                        goal_tile.GetComponent<Renderer>().material.color = Color.red; 
                    }
                    // else if( target_goals.Contains(tile_name))
                    // {
                    //     GameObject agent = field_constructor.graph[tile_name].occupant;
                    //     if (!agent.team_name.Equals(active_team))
                    //     {
                            
                    //     } 
                    //     else
                    //     {
                    //         // Debug.Log("Same ")
                    //     }
           
                    // }
                }
            }
            else
            {
                ChooseStartTile(tile_name);
            }
        }
    }

    void ChooseStartTile(string tile_name)
    {
        // Agent must be present on the tile
        if (!field_constructor.graph.ContainsKey(tile_name))
        {
            Debug.Log("dictionary does not contain "+tile_name);
            return;
        }
        GameObject agent = field_constructor.graph[tile_name].occupant; 

        if (agent != null && active_team.Equals(agent.GetComponent<AgentHandler>().team_name))
        {
            start_tile = GameObject.Find(tile_name);
            start_tile.GetComponent<Renderer>().material.color = Color.yellow; 
            // Get the speed of the agent 
            double agent_speed = agent.GetComponent<AgentHandler>().speed;
            double agent_range = agent.GetComponent<AgentHandler>().range;
            // available_goals = field_constructor.get_available_goals(start_tile.name, agent_speed);
            (available_goals, target_goals) = field_constructor.get_available_goals_and_targets(start_tile.name, 
                                                                                                agent_speed, 
                                                                                                agent_range);
            
            foreach(string available_goal_name in available_goals)
            {
                GameObject available_goal_tile = GameObject.Find(available_goal_name);
                available_goal_tile.GetComponent<Renderer>().material.color = Color.cyan;
            }
            foreach(string target_goal_name in target_goals)
            {
                GameObject target_goal_tile = GameObject.Find(target_goal_name);
                target_goal_tile.GetComponent<Renderer>().material.color = Color.red;
            }

            agent_interface_manager.agent_ui_event.Invoke(agent.name + " on " + tile_name);
        }
    }
}
