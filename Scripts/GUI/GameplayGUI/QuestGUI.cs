using UnityEngine;
using System.Collections;

public class QuestGUI : MonoBehaviour
{

    public Transform QuestList;
    public QuestItemGUI questItemPrefab;


    private void Awake()
    {
        QuestManager.Instance.QuestAdded += QuestAdded;
    }

    private void QuestAdded(Quest quest)
    {
        var questItem = Instantiate(questItemPrefab);
        questItem.Quest = quest;
        questItem.transform.SetParent(QuestList.transform);
    }

}
