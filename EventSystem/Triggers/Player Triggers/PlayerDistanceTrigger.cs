using UnityEngine;
using System.Collections;

public class PlayerDistanceTrigger : GameEventTrigger
{

    public float triggerOnDistance = 35f;
    [Tooltip("If true the y coord of both this object will not factor into determining the distance")]
    public bool negateYCoord = true;
    public bool triggerEnterEvents = true;
    public bool triggerExitEvents = true;

    private PlayerController player;

    public override void Start()
    {
        base.Start();
        player = GameMainReferences.Instance.Player;
    }

    public void Update()
    {
        Vector3 pos = negateYCoord ? new Vector3(transform.position.x, player.transform.position.y, transform.position.z) : transform.position;

        if (Vector3.Distance(pos, player.transform.position) <= triggerOnDistance)
        {
            if (triggerEnterEvents)
            {

                TriggerEnter(null);
            }
        }
        else
        {
            if (triggerExitEvents)
            {
                TriggerExit(null);
            }
        }
    }

}
