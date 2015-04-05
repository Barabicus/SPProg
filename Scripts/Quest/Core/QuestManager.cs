using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour
{

    public static QuestManager Instance { get; set; }

    public List<Quest> Quests { get; set; }

    public event Action<Quest> QuestAdded;

    private void Awake()
    {
        Quests = new List<Quest>();
        Instance = this;
    }

    public void AddQuest(Quest quest)
    {
        Quests.Add(quest);
        quest.QuestCompleted += QuestComplete;
        if (QuestAdded != null)
            QuestAdded(quest);
        quest.OnQuestAdded();
    }

    private void QuestComplete(Quest quest)
    {
        Quests.Remove(quest);
    }

    private void Update()
    {
        foreach (var quest in Quests)
        {
            quest.UpdateQuest();
        }
    }

}
