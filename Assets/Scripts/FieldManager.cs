using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AgentSpeedEvent : UnityEvent<string>
{
}

public class FieldManager : MonoBehaviour
{
    public AgentSpeedEvent agent_speed_event;

    FieldConstructor field_constructor; 
    // Start is called before the first frame update
    void Start()
    {
        field_constructor = GameObject.Find("field").GetComponent<FieldConstructor>(); 


        if (agent_speed_event == null)
        {
            agent_speed_event = new AgentSpeedEvent();
        }
    }

    void OnEnable ()
    {
        agent_speed_event.AddListener(ShowMovement);
    }   

    void OnDisable ()
    {
        agent_speed_event.RemoveListener(ShowMovement);
    }

    void ShowMovement(string agent_name)
    {
        Debug.Log(agent_name);
    }

}
