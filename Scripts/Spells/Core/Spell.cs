using UnityEngine;
using System.Collections;
using System;

public abstract class Spell : MonoBehaviour
{

    #region Properties
    public Entity CastingEntity { get; set; }

    public Transform SpellTarget
    {
        get;
        set;
    }

    public Vector3? SpellTargetPosition
    {
        get;
        set;
    }

    public abstract float SpellLiveTime
    {
        get;
    }

    public abstract SpellType SpellType
    {
        get;
    }

    #endregion

    #region States



    #endregion

    #region Events
    public event EventHandler<SpellEventargs> OnDestroy;
    #endregion

    public virtual void Start()
    {
        gameObject.layer = 10;
        Invoke("DestroySpell", SpellLiveTime);
    }

    public virtual bool RequireTarget { get { return false; } }
    public virtual bool RequireTransform { get { return false; } }

    public abstract SpellID SpellID { get; }

    public void CastSpell(Entity castingEntity)
    {
        CastingEntity = castingEntity;
    }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void ApplySpell(Entity entity) { }

    /// <summary>
    /// Called when the spell is destroyed
    /// </summary>
    protected virtual void DestroySpell()
    {
        if (OnDestroy != null)
            OnDestroy(this, new SpellEventargs(this, SpellType));
        Destroy(gameObject);
    }

}

public enum SpellType
{
    Missile,
    SelfCast
}

public class SpellEventargs : EventArgs
{
    public Spell spell;
    public SpellType spellType;

    public SpellEventargs(Spell spell, SpellType spellType)
    {
        this.spellType = spellType;
        this.spell = spell;
    }
    
}