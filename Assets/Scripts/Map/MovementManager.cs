using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{

    FieldConstructor map;
    // Start is called before the first frame update
    void Start()
    {
        map = GameObject.Find("field").GetComponent<FieldConstructor>(); 
    }

    string selected_tile = "";
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit ))
            {
                if (selected_tile.Equals(hit.collider.gameObject.name))
                {
                    // Confirm movement 
                    selected_tile = "";
                }
                else
                {
                    selected_tile = hit.collider.gameObject.name; 
                }


                // if(map.graph.ContainsKey(selected_tile))
                // {

                // }
            }
        }       
    }


}
