using UnityEngine;
using System.Collections;

public abstract class GameEvent : MonoBehaviour
{
    public GameEventTrigger[] triggers;
    public bool ownerIsTrigger = true;

    public virtual void Awake() { }
    public virtual void Start()
    {
        foreach (GameEventTrigger tr in triggers)
        {
            tr.onTrigger += TriggerEvent;
        }
        if (ownerIsTrigger)
        {
            GameEventTrigger ownerTrigger = transform.parent.GetComponent<GameEventTrigger>();

            if (ownerTrigger != null)
                ownerTrigger.onTrigger += TriggerEvent;
        }
    }
    public virtual void TriggerEvent(Collider other) { }
    public virtual void Update() { }


}
