using UnityEngine;
using System.Collections;

public abstract class GameEvent : MonoBehaviour
{
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void TriggerEvent(Collider other) { }
    public virtual void Update() { }


}
