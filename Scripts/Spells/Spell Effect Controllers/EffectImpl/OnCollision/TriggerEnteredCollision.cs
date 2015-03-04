using UnityEngine;
using System.Collections;

public class TriggerEnteredCollision : SpellEffect
{
    public bool enterCausesCollision = true;
    public bool exitCausesCollision = true;


    private void OnTriggerEnter(Collider other)
    {
        effectSetting.TriggerCollision(new ColliderEventArgs(), other);
    }

    private void OnTriggerExit(Collider other)
    {
        effectSetting.TriggerCollision(new ColliderEventArgs(), other);
    }

}
