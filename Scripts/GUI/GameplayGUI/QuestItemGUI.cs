using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestItemGUI : MonoBehaviour
{

    public Text questText;

    public Quest Quest { get; set; }

    private void Start()
    {
        questText.text = Quest.QuestDescription;
        Quest.QuestCompleted += QuestCompleted;
    }

    private void QuestCompleted(Quest quest)
    {
        questText.color = Color.green;
        questText.text = "completed";
    }

}
