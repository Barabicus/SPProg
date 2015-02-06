using UnityEngine;
using System.Collections;
using System;

public abstract class GameEventTrigger : MonoBehaviour
{
    public event Action<Collider> onTrigger;
    public bool singleFire = true;
    public float triggerDelay = 0f;

    private float _lastTrigger;

    public virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
    public virtual void Awake() { }
    
    public virtual void OnTriggered(Collider other)
    {
        if (!singleFire && Time.time - _lastTrigger <= triggerDelay)
            return;

        if (onTrigger != null)
            onTrigger(other);

        if (singleFire)
            Destroy(this);
        else
            _lastTrigger = Time.time;

    }
}
