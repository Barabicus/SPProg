using UnityEngine;
using System.Collections;

public class AreaReachedObjective : QuestObjective
{
    public float triggerDistance = 5f;

    protected override void DoObjectiveUpdate()
    {
        base.DoObjectiveUpdate();
        if (Vector3.Distance(transform.position, Player.transform.position) <= triggerDistance)
            TriggerObjectiveComplete();
    }
}
