using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EffectSetting))]
public abstract class Spell : MonoBehaviour
{

    #region Fields

    Rigidbody rigidbody;

    #endregion

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

    public Vector3 SpellStartPosition
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

    /// <summary>
    /// How much delay this spell leaves before you can cast another spell
    /// </summary>
    public abstract float SpellCastDelay
    {
        get;
    }

    #endregion

    #region States



    #endregion

    #region Events
    public event EventHandler<SpellEventargs> OnDestroy;
    public event EventHandler<SpellEventargs> OnCollided;
    #endregion

    public virtual void Awake() { }

    public virtual void Start()
    {
        // Ensure the components are properly setup
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        GetComponent<SphereCollider>().isTrigger = true;
        gameObject.layer = 10;


        Invoke("DestroySpell", SpellLiveTime);
    }

    public virtual bool RequireTarget { get { return false; } }
    public virtual bool RequireTransform { get { return false; } }

    public abstract SpellID SpellID { get; }

    public void CastSpell(Entity castingEntity, Vector3 startPostion)
    {
        SpellStartPosition = startPostion;
        CastingEntity = castingEntity;
        transform.position = SpellStartPosition;
        transform.rotation = CastingEntity.transform.rotation;
        gameObject.SetActive(true);
    }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void ApplySpell(Entity entity) { }

    /// <summary>
    /// Called when the spell is destroyed
    /// </summary>
    protected virtual void DestroySpell()
    {
        TriggerDestroyEvent();
        enabled = false;
    }


    #region Trigger Events

    protected void TriggerCollisionEvent()
    {
        if (OnCollided != null)
            OnCollided(this, new SpellEventargs(this, SpellType));
    }

    protected void TriggerDestroyEvent()
    {
        if (OnDestroy != null)
            OnDestroy(this, new SpellEventargs(this, SpellType));
    }

    #endregion

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