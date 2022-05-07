using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnGUI : MonoBehaviour
{
    List<GameObject> child_buttons = new List<GameObject>(); 
    // Start is called before the first frame update
    public Dictionary<int, List<GameObject>> layer_buttons = new Dictionary<int, List<GameObject>>();

    public Dictionary<string, GameObject> lineage = new Dictionary<string, GameObject>(); 

    void Start()
    {   
        GetChildrenRecursive(0, this.gameObject);

        // TurnOffAllButtons();
        // SetLayer(0, true);
        // right_click_tracker = true;
    }

    public void TurnOffAllButtons()
    {
        right_click_tracker = true;
        foreach(KeyValuePair<int, List<GameObject>> entry in layer_buttons)
        {
            foreach(GameObject go in entry.Value)
            {
                go.SetActive(false);
            }
        }
    }

    void SetLayer(int layer, bool active)
    {
        foreach(GameObject go in layer_buttons[layer])
        {
            go.SetActive(active);
        }
    }

    void GetChildrenRecursive(int layer, GameObject game_object)
    {
        foreach (Transform child in game_object.transform)
        {
            if(!layer_buttons.ContainsKey(layer))
            {
                layer_buttons[layer] = new List<GameObject>();
            }

            if (child.name != "Text")
            {
                layer_buttons[layer].Add(child.gameObject);
                lineage[child.gameObject.name] = child.gameObject;
                // Debug.Log(layer + " | " + child.name);
            }

            GetChildrenRecursive(layer + 1, child.gameObject);
        }
    }

    void SetChildrenRecursive(GameObject game_object, bool active)
    {
        foreach (Transform child in game_object.transform)
        {
            if (child.name != "Text")
            {
                child.gameObject.SetActive(active);
            }
            SetChildrenRecursive(child.gameObject, active);
        }
    }

    public void TurnOnChildren(string parent)
    {
        foreach(Transform child in lineage[parent].transform)
        {
            if (child.name != "Text")
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }

        if (parent.Equals("Move"))
        {
            this.Move();
        }
    }

    public delegate void EndTurnEvent ();
	public static event EndTurnEvent end_turn;
    public void EndTurn()
    {
        end_turn?.Invoke();
    }

    public delegate void MoveEvent ();
	public static event MoveEvent move_event;
    public void Move()
    {
        move_event?.Invoke();
    }

    void HideGui()
    {
        SetChildrenRecursive(this.gameObject, false);
        SetLayer(0, true);
        // right_click_tracker = false;
    } 

    void OnEnable()
    {
        end_turn += HideGui; 
    }

    void OnDisable()
    {
        end_turn -= HideGui; 
    }

    bool right_click_tracker = true;
    void Update()
    {
        // if (Input.GetMouseButtonDown(1))
        // {   
        //     this.gameObject.GetComponent<RectTransform>().anchoredPosition  = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
        //     Debug.Log(" right click tracker " + right_click_tracker);
        
        //     if(!right_click_tracker)
        //     {
        //         HideGui();
        //     }
            
        //     SetLayer(0, right_click_tracker);   
        //     right_click_tracker = !right_click_tracker;
        // }
    }
}
