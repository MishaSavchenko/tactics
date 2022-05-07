using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    private Map map;
    private TurnManager turn_manager;
    private KeypadPath path_interface;


    void OnEnable()
    {
        map = gameObject.GetComponent<Map>();   
        turn_manager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        path_interface = GameObject.Find("KeypadPath").GetComponent<KeypadPath>();
        
        TurnGUI.end_turn += map.CleanCharacterSpeed;
        TurnGUI.move_event += this.PathToggle;
    }

    void OnDisable()
    {
        TurnGUI.end_turn -= map.CleanCharacterSpeed;
        TurnGUI.move_event -= this.PathToggle;
    }

    void PathToggle()
    {
        if( map.last_movement_ != null )
        {
            map.CleanCharacterSpeed();
        }
        else
        {
            // map.ShowCharacterSpeed(turn_manager.current_character_name);
            
            path_interface.path_boundaries = map.ShowCharacterSpeed(turn_manager.current_character_name);
            string current_character_tile = map.GetCharacterLocation(turn_manager.current_character_name);
            path_interface.SetupStartTile(current_character_tile);
        }
    }


}
