using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputs : MonoBehaviour
{
    TurnManager turn_manager; 
    


    void Start()
    {
        turn_manager = GameObject.Find("TurnManager").GetComponent<TurnManager>(); 
    }

    void Update()
    {
        if(turn_manager is null)
        {
            Debug.LogWarning("turn_manager object is null");        
        }
        // 
        if (Input.GetMouseButtonDown(0)) {
            CastRay();
        }       
    }


    void CastRay() 
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit ))
        {
            string tile_name = hit.collider.gameObject.name; 
            turn_manager.goal_tile_name = tile_name; 
        }
    }
}
