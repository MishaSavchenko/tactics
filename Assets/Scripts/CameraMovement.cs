using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update


    void Start()
    {

        // Debug.Log(transform.position);
    }

    public float speed = 5.0f;
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(speed * Time.deltaTime,0,0);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-speed * Time.deltaTime,0,0);
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(0,-speed * Time.deltaTime,0);
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0,speed * Time.deltaTime,0);
        }


        Camera c = GameObject.Find("Main Camera").GetComponent<Camera>();
        c.orthographicSize += 0.1f * Input.mouseScrollDelta.y;
        if (c.orthographicSize <= 0.5)
        {
            c.orthographicSize = 0.5f;
        }
        // c.orthographicSize += 0.1f * Input.mouseScrollDelta.y;
        // Camera c = GetComponentsInChildren(typeof(Camera), true)[0]; //.orthographicSize += 0.1f * Input.mouseScrollDelta.y;
        // Vector3 zoom_transform = new Vector3(0.0f, 0.0f, Input.mouseScrollDelta.y); 
        // if(Input.mouseScrollDelta.y > 0)
        // {
        // transform.localPosition += transform.rotation * zoom_transform;  
        // }
        // else if(Input.mouseScrollDelta.y < 0)
        // {
        //     transform.localPosition *= -1.05f;  
        // }
        // transform.position += zoom_transform;
        // Debug.Log(Input.mouseScrollDelta);
        // if( Input.mouseScrollDelta)
        // {
        //     Debug.Log(Input.mouseScrollDelta);
        // }
        // pos.y += Input.mouseScrollDelta.y * scale;
    }   
}


