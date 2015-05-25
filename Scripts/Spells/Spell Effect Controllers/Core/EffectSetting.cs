using UnityEngine;
using System.Collections;
using System;

public class EffectSetting : MonoBehaviour
{

    public Spell spell;
    public bool destroyOnCollision;
    public float destroyTimeDelay = 0f;
    public event Action OnSpellStart;
    public event Action OnSpellDestroy;
    public event Action<ColliderEventArgs, Collider> OnSpellCollision;
    public event Action<Entity> OnSpellApply;
    public event Action OnEffectDestroy;
    public event Action OnSpellCast;
    public event Action OnSpellReset;

    /// <summary>
    /// This is called once to setup initial references that will persist with the EffectSetting throughout the duration of the game.
    /// </summary>
    public void InitializeEffect()
    {
        spell = GetComponent<Spell>();
        spell.OnSpellDestroy += spell_OnSpellDestroy;

        foreach (var componentsInChild in transform.GetComponentsInChildren<SpellEffect>(true))
        {
            componentsInChild.InitializeEffect(this);
        }

    }
    /// <summary>
    /// This is called just as the spell has been created. Just before the game object is set to active.
    /// </summary>
    public void TriggerSpellCast()
    {
        if (OnSpellStart != null)
            OnSpellStart();
        if (OnSpellCast != null)
            OnSpellCast();
    }


    /// <summary>
    /// Calling this will trigger a collision in all of the _spellAIProprties effects
    /// </summary>
    /// <param name="args"></param>
    /// <param name="other"></param>
    public void TriggerCollision(ColliderEventArgs args, Collider other)
    {
        if (OnSpellCollision != null)
            OnSpellCollision(args, other);
        if (destroyOnCollision)
        {
            TriggerDestroySpell();
        }
    }
    /// <summary>
    /// Trigger a spell apply to be fired. This will tell all SpellEffects to apply what ever effects they have.
    /// </summary>
    /// <param name="entity"></param>
    public void TriggerApplySpell(Entity entity)
    {
        if (OnSpellApply != null)
            OnSpellApply(entity);
    }
    /// <summary>
    /// Trigger a spell Destroy to be fired. This will force the spell to destroy itself.
    /// </summary>
    public void TriggerDestroySpell()
    {
        spell.DestroySpell();
    }
    /// <summary>
    /// This is called when the Spell itself has been destroyed. This will allow sometime for the effects to finish off
    /// any visual effects before the spell is removed from the world and returned to the pool.
    /// </summary>
    /// <param name="spell"></param>
    private void spell_OnSpellDestroy(Spell spell)
    {
        if (OnSpellDestroy != null)
            OnSpellDestroy();

        Invoke("DestroyGameObject", destroyTimeDelay);
    }

    private void DestroyGameObject()
    {
        CancelInvoke();
        if (OnEffectDestroy != null)
            OnEffectDestroy();

        gameObject.SetActive(false);

        spell.TriggerSpellReset();
        TriggerEffectReset();
    }
    /// <summary>
    /// This triggers an Effect Reset which will cause all SpellEffects to reset themselves. This is called just
    /// before the spell is returned to the pool.
    /// </summary>
    private void TriggerEffectReset()
    {
        if (OnSpellReset != null)
            OnSpellReset();
        ResetEffect();
    }
    /// <summary>
    /// This will return the spell to the pool.
    /// </summary>
    private void ResetEffect()
    {
        SpellPool.Instance.PoolSpell(spell);
        spell.OnSpellDestroy += spell_OnSpellDestroy;
    }

}