using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class EntityAnimatorController<T> : MonoBehaviour
{

    public Entity Entity { get; set; }
    public Animator Animator { get; set; }

    protected virtual void Start()
    {
        Entity = GetComponent<Entity>();
        Animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {

    }

    public virtual void PlayAnimation(T animation)
    {
    }


}
