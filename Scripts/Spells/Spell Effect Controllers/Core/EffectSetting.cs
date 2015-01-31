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
        Destroy(gameObject);
    }


}