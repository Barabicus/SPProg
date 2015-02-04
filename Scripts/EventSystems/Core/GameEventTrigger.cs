using UnityEngine;
using System.Collections;

public abstract class GameEventTrigger : MonoBehaviour
{
    public GameEvent[] onTriggerEvents;


    public virtual void Start() { }
    public virtual void Awake() { }
    
    public virtual void OnTriggered(Collider other)
    {
        foreach (GameEvent ge in onTriggerEvents)
        {
            ge.TriggerEvent(other);
        }
    }
}
