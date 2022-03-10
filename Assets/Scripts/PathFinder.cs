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

    void Start()
    {
        field_constructor = GameObject.FindObjectOfType<FieldConstructor>();
        if(field_constructor)
        {
            Debug.Log("found the field constructor");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            CastRay();
        }       
    }

    void CastRay() 
    {
        // Bit shift the index of the layer (8) to get a bit mask
        // int layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        // layerMask = ~layerMask;
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
                    start_tile.GetComponent<Renderer>().material.color = Color.white; 
                    goal_tile.GetComponent<Renderer>().material.color = Color.white; 
                    
                    start_tile = null;
                    goal_tile = null;
                }
                // if we want to confirm the action
                else if(String.Equals(tile_name, goal_tile.name))
                {
                    start_tile.GetComponent<Renderer>().material.color = Color.green; 
                    goal_tile.GetComponent<Renderer>().material.color = Color.green;
                    // Debug.Log("Confirming movement");
                    FieldEventManager.TriggerEvent ("test");
                    path = field_constructor.GetPath(start_tile.name, goal_tile.name);

                    foreach(string path_tile_name in path )
                    {
                        GameObject.Find(path_tile_name).GetComponent<Renderer>().material.color = Color.black;
                    }

                    start_tile = null;
                    goal_tile = null;
                }
                // if we want to choose a different target
                else{
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
                    start_tile = null;
                }
                else{
                    goal_tile = GameObject.Find(tile_name);
                    goal_tile.GetComponent<Renderer>().material.color = Color.red; 
                }
            }
            else{
                start_tile = GameObject.Find(tile_name);
                start_tile.GetComponent<Renderer>().material.color = Color.yellow; 
            }
        }
    }
}
