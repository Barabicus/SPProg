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
    public event Action<Entity> OnSpellApply;
    public event Action OnEffectDestroy;
    public event Action OnSpellCast;
    public event Action OnSpellReset;

    void Start()
    {
        spell = GetComponent<Spell>();
        spell.OnSpellDestroy += spell_OnSpellDestroy;
        StartCoroutine(TriggerSpellCast());
    }

    private IEnumerator TriggerSpellCast()
    {
        yield return null;
        if (OnSpellCast != null)
            OnSpellCast();
    }

    private void spell_OnSpellDestroy(object sender, SpellEventargs e)
    {
        if (OnSpellDestroy != null)
            OnSpellDestroy(sender, e);

        Invoke("DestroyGameObject", destroyTimeDelay);
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
            TriggerDestroy();
        }
    }

    public void TriggerApplySpell(Entity entity)
    {
        if (OnSpellApply != null)
            OnSpellApply(entity);
    }

    public void TriggerDestroy()
    {
        spell.DestroySpell();
        Invoke("DestroyGameObject", destroyTimeDelay);
    }

    private void DestroyGameObject()
    {
        if (OnEffectDestroy != null)
            OnEffectDestroy();

        gameObject.SetActive(false);

        TriggerSpellReset();

      //  Destroy(gameObject);
    }

    private void TriggerSpellReset()
    {
        if (OnSpellReset != null)
            OnSpellReset();
        ResetEffect();
    }

    private void ResetEffect()
    {
        Debug.Log("pool");
        SpellPool.Instance.PoolSpell(spell);
    }

}