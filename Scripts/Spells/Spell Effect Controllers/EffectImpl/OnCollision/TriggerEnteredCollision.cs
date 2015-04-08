using UnityEngine;
using System.Collections;

/// <summary>
/// Will trigger a collision event for the spell effect when a collider enters the _spellAIProprties collider trigger
/// </summary>
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
