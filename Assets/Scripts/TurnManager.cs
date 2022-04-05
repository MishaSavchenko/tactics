using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueue;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    // private List<Agent> agents; 
    // private Dictioanry<string, AgentHandler> agents; 
    // private List<GameObject> agents; 
    // private List<string> teams;

    // private UnityAction agent_listener;

    // private UnityAction turn_end_listener;

    FieldConstructor field_constructor;
    PathFinder path_finder;
    // Start is called before the first frame update
    PriorityQueue<AgentHandler> agents = new PriorityQueue<AgentHandler>(); 
    int current_agent_index = 0;
    AgentHandler current_agent = null;    
    public Transform camera_transform; 

    private FieldManager field_manager;
    private IMap map; 

    void Start()
    {
        field_constructor = gameObject.GetComponent<FieldConstructor>();
        path_finder = gameObject.GetComponent<PathFinder>();

        // fiel_constructor
        GameObject[] found_agents = GameObject.FindGameObjectsWithTag("Agent");
        for(int i=0; i < found_agents.Length; i++)
        {
            AgentHandler agent = found_agents[i].GetComponent<AgentHandler>();
            agents.Enqueue(agent.speed, agent);
        }

        current_agent = agents.AtIndex(0);

        field_manager = GameObject.Find("FieldManager").GetComponent<FieldManager>();
        map = GameObject.Find("field").GetComponent<IMap>();
        // agent_listener = new UnityAction(AgentCounter);
        // turn_end_listener = new UnityAction(TurnEndCB);
    }

    private void ChangeCameraPosition(AgentHandler current_agent)
    {
        camera_transform.position = current_agent.gameObject.transform.position - camera_transform.forward;
    }

    public void OnEndTurn()
    {
        map.CleanUpShownTiles();
        current_agent_index++;
    }

    public void OnTurnStart()
    {
        current_agent_index = current_agent_index % agents.Count;
        current_agent = agents.AtIndex(current_agent_index);        
        ChangeCameraPosition(current_agent);
        map.ShowCharacterSpeed(current_agent.code_name);

        map.current_character = current_agent.code_name;
    }

    public void OnMove()
    {
        map.ShowCharacterSpeed(current_agent.code_name);
        map.current_character = current_agent.code_name;
    }

    public void OnAction()
    {
        Debug.Log("Choose an action");
    }

    public void OnBonusAction()
    {
        Debug.Log("Choose a bonus action");
    }

    private void ActivateAgent(AgentHandler agent)
    {
        Debug.Log("Activating agent " + agent.gameObject.name);
        // Highlight tiles for movement
    }

    private void DeactivateAgent(AgentHandler agent)
    {
        Debug.Log("Deactivating agent " + agent.gameObject.name);
        // Revert the tile back to their original shader/color
    }


    public int box_height = 30;
    public int box_width = 75;
    void OnGUI()
    {
        if (GUI.Button(new Rect(90, 10, box_width, box_height), "End Turn"))
        {
            this.OnEndTurn();
            this.OnTurnStart();
        }
        if (GUI.Button(new Rect(90, 10 + box_height + 5, box_width, box_height), "Move"))
        {
            OnMove();
        }
        // if (GUI.Button(new Rect(10, 10 + 2 * (box_height + 5), box_width, box_height), "ResetSpeed"))
        // {
        //     this.ResetSpeed();
        //     this.GetSpeed();
        // }
    }


    void ActionLogic(string tile)
    {
        /**
        Move if
            tile is not occupied 
            tile is within range
            there is enough speed in the character 
        **/
        if (map.available_goals.Contains(tile))
        {
            if (!field_constructor)
            {
                field_constructor = GameObject.Find("field").GetComponent<FieldConstructor>();
            }
            string current_tile = field_constructor.GetCharacterPlacement(current_agent.gameObject.name);
            GameObject.Find(current_agent.gameObject.name).GetComponent<Character>().Move(current_tile, tile);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit ))
            {
                string tile_name = hit.collider.gameObject.name; 
                Debug.Log(tile_name);
                ActionLogic(tile_name); 
            }
        }       
    }

    // void TurnEndCB()
    // {
    //     Debug.Log("Turn has endded");
    //     string current_team = path_finder.active_team;
        
    //     int next_team_index = (field_constructor.team_names.IndexOf(current_team) + 1) % field_constructor.team_names.Count;
    //     path_finder.active_team = field_constructor.team_names[next_team_index];

    //     Debug.Log("Next team is : " + path_finder.active_team);
    // }


    // void AgentCounter ()
    // {
    //     GameObject[] found_agents = GameObject.FindGameObjectsWithTag("Agent");
    //     Debug.Log("There are " + found_agents.Length + " agents");
    // }


}
