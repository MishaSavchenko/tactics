using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PriorityQueue;

public enum TurnState {Nominal, Action, Movement}

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    public Transform camera_transform;

    Map map = null; 

    public TurnState current_state = TurnState.Nominal; 

    // Start is called before the first frame update
    void Start()
    {
        map = GameObject.Find("field").GetComponent<Map>();
        UpdateCharactersCache();
    }

    public List<string> character_order;
    int current_character_index = 0;

    void OnGUI()
    {
        // Make a background box
        float x = 10f;
        float y = 220f; 
        float width = 120f;
        float height = 200f;
        GUI.Box(new Rect(x, y, width, height), "Turn Manager Debug");

        x += 10; 
        y += 25;
        if(GUI.Button(new Rect(x, y, width - 20, 20), "GetCharacterOrder"))
        { 
            character_order = GetCharacterOrder();
            string co_str = "";
            foreach(string co in character_order)
            {
                co_str += " --> " + co;
            }
            Debug.Log(co_str);
        }

        y += 25;
        if(GUI.Button(new Rect(x, y, width - 20, 20), "MoveCharToGoal"))
        { 
            map.CleanUpLastPath();
            MoveCurrentToTile(goal_tile_name);
        }

        y += 25;
        if(GUI.Button(new Rect(x, y, width - 20, 20), "End Turn"))
        { 
            current_character_index++; 
            current_character_index = current_character_index % character_order.Count;
            string current_character = character_order[current_character_index];
            camera_transform.position = characters[current_character].gameObject.transform.position;
            current_character_name  = current_character;
        }
    }

    Dictionary<string, Character> characters = new Dictionary<string, Character>();
    
    public void UpdateCharactersCache()
    {
        GameObject[] char_gos = GameObject.FindGameObjectsWithTag("Character");
        for(int i = 0; i < char_gos.Length; i++)
        {
            GameObject go = char_gos[i];
            characters[go.name] = go.GetComponent<Character>();
        }
    }


    public List<string> GetCharacterOrder()
    {
        List<string> character_names = map.GetAllCharacterNames();
        PriorityQueue<string> character_queue = new PriorityQueue<string>(); 

        foreach(string character_name in character_names)
        {
            if(!characters.ContainsKey(character_name))
            {
                characters[character_name] = GameObject.Find(character_name).GetComponent<Character>();
            }
            Character char_ = characters[character_name];
            character_queue.Enqueue(char_.initiative, character_name);
        }
        return character_queue.GetList();
    } 

    public string current_character_name = "";
    public string goal_tile_name = "";

    public void MoveCurrentToTile(string new_tile_name)
    {
        if (characters.ContainsKey(current_character_name))
        {
            string current_tile_name = map.GetCharacterLocation(current_character_name);
            Debug.Log(current_tile_name);
            List<string> path = map.GetPath(current_tile_name, new_tile_name); 
            List<Vector3> path_points = map.TilesToPositions(path);
            map.ShowPath(path);
            map.MoveCharacterToTile(current_character_name, new_tile_name);
            // 
            GameObject.Find(current_character_name).transform.position = path_points[path_points.Count - 1];
        }   
        else
        {
            Debug.Log("Character [ " + current_character_name + " ] doesnt exists");
        }
    }



}
