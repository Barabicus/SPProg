using UnityEngine;
using System.Collections;
using System;

public class EffectSetting : MonoBehaviour
{

    public Spell spell;
    public bool destroyOnCollision;
    public float destroyTimeDelay = 0f;
    public event EventHandler<SpellEventargs> OnSpellDestroy;
    public event Action<ColliderEventArgs, Collider> OnSpellCollision;
    public event Action OnEffectDestroy;

    void Start()
    {
        spell = GetComponent<Spell>();
        spell.OnSpellDestroy += spell_OnSpellDestroy;
    }

    void spell_OnSpellDestroy(object sender, SpellEventargs e)
    {
        if (OnSpellDestroy != null)
            OnSpellDestroy(sender, e);

        Invoke("DestroyGameObject", destroyTimeDelay);
    }

    /// <summary>
    /// Calling this will trigger a collision in all of the spells effects
    /// </summary>
    /// <param name="args"></param>
    /// <param name="other"></param>
    public void TriggerCollision(ColliderEventArgs args, Collider other)
    {
        spell.CollisionEvent(other);
        if (OnSpellCollision != null)
            OnSpellCollision(args, other);
        if (destroyOnCollision)
        {
            spell.DestroySpell();
            Invoke("DestroyGameObject", destroyTimeDelay);
        }
    }


    private void DestroyGameObject()
    {
        if (OnEffectDestroy != null)
            OnEffectDestroy();
        Destroy(gameObject);
    }


}