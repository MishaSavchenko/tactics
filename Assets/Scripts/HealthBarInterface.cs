using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

// [System.Serializable]
// public class HealthChangeEvent : UnityEvent<double> {}

public class HealthBarInterface : MonoBehaviour
{
    public float health_fraction = 1.0f; 
    private GameObject health_bar; 
    private UnityAction health_change_listener;

    // Start is called before the first frame update
    void Start()
    {
        health_bar = GameObject.Find("health") as GameObject;
        health_change_listener = new UnityAction(UpdateHealthBar);
    }

    void OnEnable ()
    {
        AgentEventManager.StartListening ("update_health_bar", UpdateHealthBar);
    }   

    void OnDisable ()
    {
        AgentEventManager.StopListening ("update_health_bar", UpdateHealthBar);
    }

    void UpdateHealthBar()
    {
        Debug.Log(health_fraction);
        Vector3 current_scale = health_bar.transform.localScale;
        current_scale.x = health_fraction;
        health_bar.transform.localScale = current_scale;
    }
}
