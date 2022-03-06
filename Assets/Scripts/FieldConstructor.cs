using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldConstructor : MonoBehaviour
{
    public int cells = 10;

    public int grid_width = 10; 
    public int grid_height = 10;  

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
        for (int i=0 ; i < grid_width; i++)
        {
            for (int j=0 ; j < grid_height; j++ )
            {
                GameObject new_tile =  (GameObject)Instantiate (tile,
                            Vector3.right * i + Vector3.forward * j, 
                            Quaternion.identity);
                new_tile.transform.localScale *= 0.95f;
                new_tile.transform.parent = this.gameObject.transform;
                new_tile.name = i.ToString() + "_" + j.ToString(); 
                // new_tile.AddComponent<BoxCollider>();

                tiles.Add(new_tile); 
            }
        }
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
        // string[] split = start_node.Split("_"); 
        GameObject start_node = tiles[TileNameToIndex(start_node_name)];
        GameObject goal_node = tiles[TileNameToIndex(goal_node_name)];
        
        var (start_i, start_j) = TileNameToTuple(start_node_name);
        var (goal_i, goal_j) = TileNameToTuple(goal_node_name);

        List<string> path = new List<string>(); 
        
        for(int i = start_i; i <= goal_i; i++)
        {
            string tile_name = i.ToString() + "_" + start_j.ToString();             
            path.Add(tile_name);
        }
        for(int j = start_j; j <= goal_j; j++)
        {
            string tile_name = goal_i.ToString() + "_" + j.ToString(); 
            path.Add(tile_name);
        }
        return path; 
    }


    void OnEnable ()
    {
        // FieldEventManager.StartListening ("test", someListener);
    }

    
    void OnDisable ()
    {
        // FieldEventManager.StopListening ("test", someListener);
    }

    void SomeFunction ()
    {
        Debug.Log ("Some Function was called!");
    }

    GameObject start, goal; 
    void Update()
    {
    }
}
