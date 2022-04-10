using System;   
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using PriorityQueue;
using System.IO;
using System.Linq;

public class MapConstructor : MonoBehaviour
{
    public int cells = 10;
    public int grid_width = 10; 
    public int grid_height = 10;  
    public GameObject default_tile;
    public GameObject default_agent;
    
    public int number_of_agents = 4;

    public Dictionary<string, Node> graph; 

    List<Vector3> directions = new List<Vector3>{
        Vector3.forward,
        Vector3.back, 
        Vector3.left, 
        Vector3.right
    };

    private List<GameObject> loaded_tiles_;

    public List<GameObject> GetGameObjectsFromDirectory(string directory_path)
    {
        var tiles = Resources.LoadAll(directory_path, typeof(GameObject));
        List<GameObject> loaded_tiles = new List<GameObject>();
        foreach(var tile in tiles)
        {
            loaded_tiles.Add((GameObject)tile);
        }
        return loaded_tiles;
    }

    public List<string> team_names = new List<string>{"lost", "stalkers"};  
    public List<string> agent_names = new List<string>{"Saera", "Brae", "Xerxes", "Piotr"};  

    public Dictionary<string, string> placements; 


    public string GetCharacterPlacement(string character_name)
    {
        if (placements.ContainsKey(character_name))
        {
            return placements[character_name];
        }
        else{ 
            return "";
        }
    }

    void Awake()
    {
        placements = new Dictionary<string, string>(); 

        loaded_tiles_ = GetGameObjectsFromDirectory("tiles");

        graph = new Dictionary<string, Node>(); 
        // Construct the field
        for (int i=0 ; i < grid_width; i++)
        {
            for (int j=0 ; j < grid_height; j++ )
            {
                Vector3 node_position = Vector3.right * i + Vector3.forward * j;
                GameObject new_tile = null;
                if(loaded_tiles_.Count != 0)
                {
                    int random_tile_index = UnityEngine.Random.Range(0, loaded_tiles_.Count);
                    new_tile = (GameObject)Instantiate (loaded_tiles_[random_tile_index],
                                                        node_position, 
                                                        Quaternion.identity);
                    new_tile.AddComponent<BoxCollider>();                                                        

                }
                else{
                    new_tile = (GameObject)Instantiate (default_tile,
                                                                node_position, 
                                                                Quaternion.identity);
                }                

                new_tile.transform.localScale *= 0.95f;
                new_tile.transform.parent = this.gameObject.transform;
                new_tile.name = PositionToName(node_position); 
                double node_cost = UnityEngine.Random.value;
                // new_tile.GetComponent<Renderer>().material.color = Color.magenta * (float)node_cost;
                Node new_node = new Node(i, j, new_tile, 5.0);
                graph[new_tile.name] = new_node; 
            }
        }

        // Connect nodes with neighbors
        foreach (KeyValuePair<string, Node> name_node in graph)
        {
            Vector3 node_position = name_node.Value.game_object.transform.position;
            foreach (Vector3 direction in directions)
            {
                Vector3 neighbor_node_position =node_position + direction;
                string neighbor_name = PositionToName(neighbor_node_position);
                if(graph.ContainsKey(neighbor_name))
                {
                    name_node.Value.neighbors.Add(graph[neighbor_name]);
                    name_node.Value.edge_weights.Add(graph[neighbor_name].cost);
                }
            } 
        }
        
        // // Create agents
        // for(int i = 0; i < number_of_agents; i++)
        // {
        //     int random_tile_index = UnityEngine.Random.Range(0, graph.Count);
        //     KeyValuePair<string, Node> name_node = graph.ElementAt(random_tile_index);

        //     GameObject new_agent = (GameObject)Instantiate(default_agent,
        //                                                    name_node.Value.game_object.transform.position, 
        //                                                    Quaternion.identity);
        //     new_agent.GetComponent<AgentHandler>().team_name = team_names[i % team_names.Count];  
        //     if (new_agent.GetComponent<AgentHandler>().team_name == "stalkers")
        //     {
        //         new_agent.GetComponent<Renderer>().material.color = Color.grey;
        //     }
        //     new_agent.name = agent_names[i];
        //     new_agent.GetComponent<AgentHandler>().code_name = agent_names[i]; 
        //     name_node.Value.occupant = new_agent;

        //     //  
        //     Character character = new_agent.GetComponent<Character>(); 
        //     character.character_name = agent_names[i];
        //     placements.Add(character.character_name, name_node.Key);            

        // }

        // gameObject.GetComponent<PathFinder>().active_team = team_names[0];

        // GameObject[] agent_uis = GameObject.FindGameObjectsWithTag("AgentControl");
        // Debug.Log(agent_uis.Length);
    }

    private string PositionToName(Vector3 pos)
    {
        return ((int)pos.x).ToString() + "_" + ((int)pos.y).ToString() + "_" + ((int)pos.z).ToString();
    }

    private Vector3 NameToPosition(string name)
    {
        string[] xyz = name.Split('_');
        return new Vector3(float.Parse(xyz[0]), float.Parse(xyz[1]), float.Parse(xyz[2])); 
    }

    public List<string> GetPath(string start_node_name, string goal_node_name)
    {
        return a_star_search(start_node_name, goal_node_name);
    }

    public List<Vector3> GetVector3Path(string start_node_name, string goal_node_name)
    {
        List<string> string_path = this.GetPath(start_node_name, goal_node_name);
        List<Vector3> path = new List<Vector3>();
        foreach(string p in string_path)
        {   
            path.Add(GameObject.Find(p).transform.position);
        }
        return path; 
    }

    List<string> breadth_first_search(string start_node_name, string goal_node_name)
    {
        Queue<string> frontier = new Queue<string>();
        frontier.Enqueue(start_node_name);
        OrderedDictionary came_from = new OrderedDictionary(); 
        came_from[start_node_name] = null;
        
        while (frontier.Count != 0 )
        {
            string current_node_name = frontier.Dequeue();
            if (String.Equals(current_node_name, goal_node_name))
            {
                break;
            }
            List<Node> neighbors = graph[current_node_name].neighbors;
            foreach(Node next_node in neighbors)
            {
                if(!came_from.Contains(next_node.game_object.name) )
                {
                    frontier.Enqueue(next_node.game_object.name);
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
        return path;
    }

    List<string> dijkstra_search(string start_node_name, 
                                 string goal_node_name)
    {
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

        List<string> path = new List<string>();
        string current = goal_node_name; 
        while (!String.Equals(current, start_node_name))
        {
            path.Add(current);
            current = came_from[current].ToString();
        }
        path.Add(start_node_name);
        path.Reverse();
        return path;
    }


    public List<string> get_available_goals(string start_node_name, 
                                                  double cutoff_cost)
    {
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

    public (List<string>, List<string>) get_available_goals_and_targets(
                                                            string start_node_name, 
                                                            double speed, 
                                                            double range)
    {
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

                if(new_cost > speed + range)
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

        List<string> movement_goals = new List<string>();
        List<string> target_goals = new List<string>(); 
        foreach(DictionaryEntry node_cost in cost_so_far)
        {
            if((double)node_cost.Value <= speed)
            {
                movement_goals.Add(node_cost.Key.ToString());
            }
            else if((double)node_cost.Value <= speed + range)
            {
                target_goals.Add(node_cost.Key.ToString());
            }
        }

        return (movement_goals, target_goals);
    }

    double heuristic(Node a, Node b)
    {
        Vector3 difference = a.game_object.transform.position - b.game_object.transform.position;
        return difference.magnitude;
    }


    List<string> a_star_search(string start_node_name, 
                         string goal_node_name)
    {
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
                    double priority = new_cost + heuristic(next_node, goal_node); 
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
        return path;
    }




}