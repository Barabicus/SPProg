using UnityEngine;
using System.Collections;

public class GodTalkPlayFinishedTrigger : GameEventTrigger
{

    public override void Start()
    {
        base.Start();
        GodTalkGUI.instance.textFinished += instance_textFinished;
    }

    void instance_textFinished()
    {
        TriggerEnter(null);
    }

}
