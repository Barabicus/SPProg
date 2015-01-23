using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Spell))]
public class EffectSetting : MonoBehaviour
{

    public Spell spell;
    public bool destroyOnCollision;
    public float destroyTimeDelay = 0f;

    public bool destroyTriggerEvent = true;
    public bool collideTriggersEvent;

    /// <summary>
    /// Called when a spell destroy is triggered
    /// </summary>
    public event Action OnSpellDestroy;


    void Start()
    {
        spell = GetComponent<Spell>();

        if (destroyTriggerEvent)
            spell.OnDestroy += SpellDestroyed;

        if (collideTriggersEvent)
            spell.OnCollided += SpellDestroyed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void SpellDestroyed(object sender, SpellEventargs e)
    {
        if (OnSpellDestroy != null)
            OnSpellDestroy();
        Invoke("DestroyGameObject", destroyTimeDelay);

    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }


}