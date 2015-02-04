using UnityEngine;
using System.Collections;

public class EntityEnteredTrigger : GameEventTrigger
{
    public Entity[] triggerableEntities;
    public TriggeredBy triggeredBy = TriggeredBy.All;
    public bool singleFire = true;
    public float triggerDelay = 0f;

    private float _lastTrigger;
    public enum TriggeredBy
    {
        All,
        Select
    }

    public override void Start()
    {
        base.Start();
        _lastTrigger = Time.time;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!singleFire && Time.time - _lastTrigger <= triggerDelay)
            return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Entity"))
            return;

        switch (triggeredBy)
        {
            case TriggeredBy.All:
                TriggerSpawn(other);
                break;
            case TriggeredBy.Select:
                foreach (Entity e in triggerableEntities)
                {
                    if (e.gameObject == other.gameObject)
                    {
                        TriggerSpawn(other);
                        return;
                    }
                }
                break;
        }
    }

    private void TriggerSpawn(Collider other)
    {
        OnTriggered(other);

        if (singleFire)
            Destroy(gameObject);
        else
            _lastTrigger = Time.time;

    }
}
