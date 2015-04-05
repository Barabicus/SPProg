using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KillEntitiesObjective : QuestObjective
{
    public int killAmount = 1;
    public Entity[] targetEntityIDs;

    private List<string> _targetIDs;
    private int _currentKillAmount = 0;

    public override void OnObjectiveAdded()
    {
        base.OnObjectiveAdded();
        _targetIDs = new List<string>();
        foreach (var ent in targetEntityIDs)
        {
            if(!_targetIDs.Contains(ent.EntityID) &&  ent.EntityID != "")
                _targetIDs.Add(ent.EntityID);
        }
        EntityManager.Instance.EntityKilled += EntityKilled;
        CheckForCompletion();
    }

    private void EntityKilled(Entity e)
    {
        if (_targetIDs.Contains(e.EntityID))
        {
            _currentKillAmount++;
            CheckForCompletion();
        }
    }

    private void CheckForCompletion()
    {
        if (killAmount == _currentKillAmount)
        {
            TriggerObjectiveComplete();
            EntityManager.Instance.EntityKilled -= EntityKilled;
        }
    }
}
