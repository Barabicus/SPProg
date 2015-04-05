using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Quest : MonoBehaviour
{

    [SerializeField] private string _questDescription;

    public string QuestDescription { get { return _questDescription;} }

    public List<QuestObjective> QuestObjectives { get; set; }

    public event Action<Quest> QuestCompleted;

    public int NumberOfObjectives
    {
        get { return QuestObjectives.Count; }
    }

    private int _objectivesCompleted;


    public void OnQuestAdded()
    {
        QuestObjectives = new List<QuestObjective>();

        foreach (var objective in GetComponentsInChildren<QuestObjective>())
        {
            objective.ObjectiveCompleted += ObjectiveCompleted;
            objective.OnObjectiveAdded();
            QuestObjectives.Add(objective);
        }

        CheckForCompletion();
    }

    public void UpdateQuest()
    {
        foreach (var objective in QuestObjectives)
        {
            objective.UpdateQuestObjective();
        }
    }

    private void ObjectiveCompleted(QuestObjective quest)
    {
        _objectivesCompleted++;
        CheckForCompletion();
    }

    private void CheckForCompletion()
    {
        if (_objectivesCompleted == NumberOfObjectives && QuestCompleted != null)
            QuestCompleted(this);
    }

}
