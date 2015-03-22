using UnityEngine;
using System.Collections;

public class GodTalkPlayTextEvent : GameEvent
{
    public string[] playText;

    public override void EnterEvent(Collider other)
    {
        base.EnterEvent(other);
        foreach (string s in playText)
            GodTalkGUI.instance.QueueText(s);
    }

}
