using System;   
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using PriorityQueue;
public class Node
{
    public int i, j;
    public GameObject game_object;  
    public double cost; 

    public List<Node> neighbors;
    public List<double> edge_weights;

    public Node(int i_, int j_, GameObject game_object_, double cost_)
    {
        i = i_;
        j = j_;
        game_object = game_object_;
        cost = cost_; 
        neighbors = new List<Node>(); 
        edge_weights = new List<double>(); 
    }
}


public class FieldConstructor : MonoBehaviour
{
    public int cells = 10;
    public int grid_width = 10; 
    public int grid_height = 10;  

    public Dictionary<string, Node> graph; 

    List<Vector3> directions = new List<Vector3>{
        Vector3.forward,
        Vector3.back, 
        Vector3.left, 
        Vector3.right
    };

    public GameObject tile;
    List<GameObject> tiles = new List<GameObject>() ;

    void Start()
    {
        graph = new Dictionary<string, Node>(); 
        // UnityEngine.Random rand = new UnityEngine.Random();
        for (int i=0 ; i < grid_width; i++)
        {
            for (int j=0 ; j < grid_height; j++ )
            {
                Vector3 node_position = Vector3.right * i + Vector3.forward * j;
                
                GameObject new_tile = (GameObject)Instantiate (tile,
                                                            node_position, 
                                                            Quaternion.identity);

                new_tile.transform.localScale *= 0.95f;
                new_tile.transform.parent = this.gameObject.transform;
                new_tile.name = PositionToName(node_position); 
                double node_cost = UnityEngine.Random.value;
                new_tile.GetComponent<Renderer>().material.color = Color.magenta * (float)node_cost;
                Node new_node = new Node(i, j, new_tile, node_cost);
                graph[new_tile.name] = new_node; 


                tiles.Add(new_tile);
            }
        }

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

    private string ijToName(int i, int j)
    {
        return i.ToString() + "_" + j.ToString();
    }

    private int TileNameToIndex(string tile_name)
    {
        var tile_tuple = TileNameToTuple(tile_name);
        return tile_tuple.Item1 * grid_height + tile_tuple.Item2;
    }

    private (int, int) TileNameToTuple(string tile_name)
    {
        string[] substrings = tile_name.Split('_');
        return (int.Parse(substrings[0]) , int.Parse(substrings[1]));
    } 

    public List<string> GetPath(string start_node_name, string goal_node_name)
    {
        return dijkstra_search(start_node_name, goal_node_name); 
    }

    void Update()
    {
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
                    Debug.Log(new_cost);
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
}