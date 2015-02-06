using UnityEngine;
using System.Collections;

public class EntityEnteredTrigger : GameEventTrigger
{
    public Entity[] triggerableEntities;
    public TriggeredBy triggeredBy = TriggeredBy.All;

    public enum TriggeredBy
    {
        All,
        Select
    }

    public override void Start()
    {
        base.Start();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Entity"))
            return;

        switch (triggeredBy)
        {
            case TriggeredBy.All:
                OnTriggered(other);
                break;
            case TriggeredBy.Select:
                foreach (Entity e in triggerableEntities)
                {
                    if (e.gameObject == other.gameObject)
                    {
                        OnTriggered(other);
                        return;
                    }
                }
                break;
        }
    }
}
