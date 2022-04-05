using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Attributes attributes; 

    public double initiative; 
    
    public string character_name; 
    public string job; 
    public string background; 
    public string race;
    [SerializeField]
    private string level; 

    // Start is called before the first frame update
    void Start()
    {
        attributes = new Attributes(); 
    }

    public double GetSpeed()
    {
        var current_speed = attributes.combat_attributes["speed"]; 
        Debug.Log(current_speed); 
        return current_speed.current; 
    }

    public void UseSpeed(double used_speed=5)
    {
        attributes.combat_attributes["speed"] -= used_speed;
    }

    public void AddSpeed(double added_speed=5)
    {
        attributes.combat_attributes["speed"] += added_speed;
    }

    public void ResetSpeed()
    {
        attributes.combat_attributes["speed"].Reset();
    }

    public int box_height = 30;
    public int box_width = 75;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, box_width, box_height), "GetSpeed"))
        {
            this.GetSpeed();
        }
        if (GUI.Button(new Rect(10, 10 + box_height + 5, box_width, box_height), "UseSpeed"))
        {
            this.UseSpeed();
            this.GetSpeed();
        }
        if (GUI.Button(new Rect(10, 10 + 2 * (box_height + 5), box_width, box_height), "ResetSpeed"))
        {
            this.ResetSpeed();
            this.GetSpeed();
        }
        // if (GUI.Button(new Rect(10, 10 + 3 * (box_height + 5), box_width, box_height), "Move"))
        // {
        //     this.ResetSpeed();
        //     this.GetSpeed();
        // }
    }

    // How to track speed while moving  


    public string start_tile_name_ = ""; 
    public string goal_tile_name_ = ""; 

    public void Move(string start_tile_name, string goal_tile_name) 
    {
        FieldConstructor map = GameObject.Find("field").GetComponent<FieldConstructor>();
        List<Vector3> path = map.GetVector3Path(start_tile_name, goal_tile_name);
        GetComponent<AgentHandler>().GeneratePathTrajectory(path);
    }

    public void TurnReset()
    {
        this.ResetSpeed();
    }



}

