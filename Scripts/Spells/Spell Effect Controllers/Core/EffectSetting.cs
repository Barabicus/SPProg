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

    public void InitializeEffect()
    {
        spell = GetComponent<Spell>();
        spell.OnSpellDestroy += spell_OnSpellDestroy;

        foreach (var componentsInChild in transform.GetComponentsInChildren<SpellEffect>(true))
        {
            componentsInChild.InitializeEffect(this);
        }

    }

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

    public void TriggerApplySpell(Entity entity)
    {
        if (OnSpellApply != null)
            OnSpellApply(entity);
    }

    public void TriggerDestroySpell()
    {
        spell.DestroySpell();
    }


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

      //  Destroy(gameObject);
    }

    private void TriggerEffectReset()
    {
        if (OnSpellReset != null)
            OnSpellReset();
        ResetEffect();
    }

    private void ResetEffect()
    {
        SpellPool.Instance.PoolSpell(spell);
        spell.OnSpellDestroy += spell_OnSpellDestroy;
    }

}