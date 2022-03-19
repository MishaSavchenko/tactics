using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class FieldEventTest : MonoBehaviour {

    private UnityAction someListener;

    void Awake ()
    {
        someListener = new UnityAction (SomeFunction);
    }

    void OnEnable ()
    {
        FieldEventManager.StartListening ("test", someListener);
        FieldEventManager.StartListening ("Spawn", SomeOtherFunction);
        FieldEventManager.StartListening ("Destroy", SomeThirdFunction);
    }

    void OnDisable ()
    {
        FieldEventManager.StopListening ("test", someListener);
        FieldEventManager.StopListening ("Spawn", SomeOtherFunction);
        FieldEventManager.StopListening ("Destroy", SomeThirdFunction);
    }

    void SomeFunction ()
    {
        // Debug.Log ("Some Function was called!");
    }

    void SomeOtherFunction ()
    {
        Debug.Log ("Some Other Function was called!");
    }

    void SomeThirdFunction ()
    {
        Debug.Log ("Some Third Function was called!");
    }
}
