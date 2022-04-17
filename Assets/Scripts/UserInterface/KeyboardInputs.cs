using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputs : MonoBehaviour
{
    public Transform cursor_transform;
    private Vector3 last_position;
    // Start is called before the first frame update
    void Start()
    {
        last_position =     GameObject.Find("0_0_0").transform.position; 
    }

    public float mod = 1.0f; 
    // Update is called once per frame
    // double last_time =
    public double time_past = 0.0;
    public double time_cut = 0.0;
    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cursor_transform.position += Vector3.left;
            Debug.Log("Left");
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            cursor_transform.position += Vector3.right;
            Debug.Log("Right");
        }
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            cursor_transform.position += Vector3.forward;
            Debug.Log("Up");
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursor_transform.position += Vector3.back;
            Debug.Log("Down");
        }


        if (time_past > time_cut)
        {
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                cursor_transform.position += Vector3.left ;
                Debug.Log("Left");
            }
            if(Input.GetKey(KeyCode.RightArrow))
            {
                cursor_transform.position += Vector3.right;
                Debug.Log("Right");
            }
            if(Input.GetKey(KeyCode.UpArrow))
            {
                cursor_transform.position += Vector3.forward;
                Debug.Log("Up");
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                cursor_transform.position += Vector3.back;
                Debug.Log("Down");
            }

            time_past = 0.0;
        }
        else
        {
            time_past += Time.deltaTime;
        }

    }
}

            // if(Input.GetKey(KeyCode.LeftArrow))
            // {
            //     cursor_transform.position += Vector3.left * (Time.deltaTime * mod);
            //     // Vector3Int t = cursor_transform.position;
            //     Debug.Log("Left");
            // }
            // if(Input.GetKey(KeyCode.RightArrow))
            // {
            //     cursor_transform.position += Vector3.right * (Time.deltaTime * mod);
            //     Debug.Log("Right");
            // }
            // if(Input.GetKey(KeyCode.UpArrow))
            // {
            //     cursor_transform.position += Vector3.forward * (Time.deltaTime * mod);
            //     Debug.Log("Up");
            // }
            // if(Input.GetKey(KeyCode.DownArrow))
            // {
            //     cursor_transform.position += Vector3.back * (Time.deltaTime * mod);
            //     Debug.Log("Down");
            // }