using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(EffectSetting))]
public class Spell : MonoBehaviour
{

    #region Fields

    Rigidbody rigidbody;

    public string spellID;
    public string spellName = "NOTSET";
    public float spellLiveTime;
    public float spellCastDelay;
    public SpellType spellType;
    public EntityAnimation spellAnimation = EntityAnimation.Nothing;
    public ElementalStats elementalCost;
    public SpellElementType elementType = SpellElementType.NoElement;
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

    /// <summary>
    /// The start position of the entity
    /// </summary>
    public Vector3 SpellStartPosition
    {
        get;
        set;
    }

    /// <summary>
    /// The position where the spell was started. Note this is not usually the entity
    /// more so the casting hand for example.
    /// </summary>
    public Transform SpellStartTransform
    {
        get;
        set;
    }

    public ElementalStats ElementalCost
    {
        get { return elementalCost; }
    }

    public virtual float SpellLiveTime
    {
        get { return spellLiveTime; }
    }

    public SpellType SpellType
    {
        get { return spellType; }
    }

    /// <summary>
    /// How much delay this spell leaves before you can cast another spell
    /// </summary>
    public virtual float SpellCastDelay
    {
        get { return spellCastDelay; }
    }

    /// <summary>
    /// Creates a transform when the spell is created. This transform will be associated with
    /// SpellTarget when the spell is destroyed this transform is also destroyed.
    /// </summary>
    public virtual bool CreateTransformOnLoad
    {
        get { return false; }
    }

    public string SpellID
    {
        get { return spellID; }
    }


    #endregion

    #region States



    #endregion

    #region Events

    public event EventHandler<SpellEventargs> OnSpellDestroy;

    #endregion


    public virtual void Awake() { }

    public virtual void Start()
    {
        gameObject.layer = 10;
        Invoke("DestroySpell", SpellLiveTime);
    }

    public void CastSpell(Entity castingEntity, Transform startPosition)
    {
        CastingEntity = castingEntity;
        SpellStartTransform = startPosition;
        SpellStartPosition = startPosition.position;
        transform.position = SpellStartPosition;
        transform.rotation = CastingEntity.transform.rotation;
        if (CreateTransformOnLoad)
            SpellTarget = new GameObject("Target: " + gameObject.name).transform;
        gameObject.SetActive(true);
    }


    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void ApplySpell(Entity entity)
    {
        entity.SpellCastBy(new SpellEventargs(this));
    }

    /// <summary>
    /// Called when the spell is destroyed
    /// </summary>
    public virtual void DestroySpell()
    {
        DestroyEvent();
        enabled = false;
        // Destroy the target transform if it was one that was created on load
        if (CreateTransformOnLoad)
            Destroy(SpellTarget.gameObject);
    }


    #region Trigger Events

    /// <summary>
    /// Called when a collision is triggered.
    /// </summary>
    /// <param name="other"></param>
    public virtual void CollisionEvent(Collider other)
    {
    }

    /// <summary>
    /// Called when the spell is destroyed.
    /// </summary>
    public virtual void DestroyEvent()
    {
        if (OnSpellDestroy != null)
            OnSpellDestroy(this, new SpellEventargs(this));
    }

    #endregion

}

public enum SpellType
{
    Missile,
    SelfCast,
    Beam,
    Physical,
    Attached,
    Area
}

public enum SpellElementType
{
    NoElement,
    Physical,
    Fire,
    Water,
    Air,
    Earth,
}

public class SpellEventargs : EventArgs
{
    public Spell spell;

    public SpellEventargs(Spell spell)
    {
        this.spell = spell;
    }

}