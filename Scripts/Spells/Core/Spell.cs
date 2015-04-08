using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(EffectSetting))]
public class Spell : MonoBehaviour
{

    #region Fields

    public string spellID;
    public string spellName = "NOTSET";
    public float spellLiveTime;
    public float spellCastDelay;
    public SpellType spellType;
    [SerializeField]
    private SpellDeathMarker _spellDeathMarker = SpellDeathMarker.None;
    public AudioClip castAudio;
    public Sprite spellIcon;
    public string spellDescription = "NOT IMPLEMENTED";
    public ElementalStats elementalCost;
    public SpellElementType elementType = SpellElementType.NoElement;
    #endregion

    #region Properties
    public Entity CastingEntity { get; set; }

    public List<Entity> IgnoreEntities { get; set; } 

    public SpellDeathMarker SpellDeathMarker
    {
        get { return _spellDeathMarker;}
    }

    public string SpellDescription
    {
        get { return spellDescription; }
    }

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


    public virtual void Awake() 
    {
        IgnoreEntities = new List<Entity>();
    }

    public virtual void Start()
    {
        gameObject.layer = 10;
        Invoke("DestroySpell", SpellLiveTime);
    }

    public void CastSpell(Entity castingEntity)
    {
        CastingEntity = castingEntity;
    }

    public void SetupSpellTransform(Transform startPosition)
    {
        SpellStartTransform = startPosition;
        SpellStartPosition = startPosition.position;
        transform.position = SpellStartPosition;
        transform.rotation = CastingEntity.transform.rotation;
        gameObject.SetActive(true);
    }

    public void SetupSpellVector(Vector3 startPosition)
    {
        SpellStartPosition = startPosition;
        transform.position = SpellStartPosition;
        transform.rotation = CastingEntity.transform.rotation;
        gameObject.SetActive(true);
    }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    /// <summary>
    /// Called when the spell is destroyed
    /// </summary>
    public void DestroySpell()
    {
        DestroyEvent();
        enabled = false;
    }


    #region Trigger Events

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

public enum SpellDeathMarker
{
    None,
    Explode,
    Freeze,
    Burn
}

public class SpellEventargs : EventArgs
{
    public Spell spell;

    public SpellEventargs(Spell spell)
    {
        this.spell = spell;
    }

}