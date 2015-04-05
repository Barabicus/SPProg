using UnityEngine;
using System.Collections;
using System;

public abstract class QuestObjective : MonoBehaviour
{

    [SerializeField]
    private string _objectiveDescription;

    public string QuestDescription { get { return _objectiveDescription; } }

    public Quest Quest { get; set; }
    public bool ObjectiveComplete { get; private set; }


    protected PlayerController Player
    {
        get { return GameMainReferences.Instance.Player; }
    }

    public event Action<QuestObjective> ObjectiveCompleted;


    public virtual void OnObjectiveAdded()
    {
        Quest = GetComponentInParent<Quest>();
    }

    public void UpdateQuestObjective()
    {
        if (!ObjectiveComplete)
            DoObjectiveUpdate();
    }

    protected virtual void DoObjectiveUpdate()
    {

    }

    protected void TriggerObjectiveComplete()
    {
        ObjectiveComplete = true;

        if (ObjectiveCompleted != null)
            ObjectiveCompleted(this);
    }

}
