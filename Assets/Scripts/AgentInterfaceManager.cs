using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AgentUIEvent : UnityEvent<string>
{
}


public class AgentInterfaceManager : MonoBehaviour
{

    private UnityAction turn_end_listener;
    public AgentUIEvent agent_ui_event;

    // GameObject[] agent_uis; 

    // Start is called before the first frame update
    void Start()
    {
        if (agent_ui_event == null)
        {
            agent_ui_event = new AgentUIEvent();
        }
        // GameObject[] agent_uis = GameObject.FindGameObjectsWithTag("AgentControl");
        // Debug.Log(agent_uis.Length);
        // agent_uis = GameObject.FindGameObjectsWithTag("AgentControl");
        // Debug.Log(agent_uis.Length);
        // foreach(GameObject ui in agent_uis)
        // {
        //     Debug.Log(ui.name);
        //     ui.SetActive(false);
        // }
    }   

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable ()
    {

        agent_ui_event.AddListener(ActivateUICB);
    }   

    void OnDisable ()
    {
        agent_ui_event.RemoveListener(ActivateUICB);
    }

    void ActivateUICB(string ui_name)
    {
        Debug.Log(ui_name);
    }

}
