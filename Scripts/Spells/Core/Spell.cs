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
    /// <summary>
    /// This is the Entity that triggered the creation of the spell
    /// </summary>
    public Entity CastingEntity { get; set; }
    /// <summary>
    /// This is a list of Entities that the Spell will ignore collisions with
    /// </summary>
    public List<Entity> IgnoreEntities { get; set; }
    /// <summary>
    /// This is the effect setting associated with this spell
    /// </summary>
    public EffectSetting SpellEffectSetting { get; set; }
    /// <summary>
    /// This is the death marker that the spell will leave when it is the spell that has killed an Entity. Eg burned, frozen etc
    /// </summary>
    public SpellDeathMarker SpellDeathMarker
    {
        get { return _spellDeathMarker; }
    }
    /// <summary>
    /// This is a brief description of the spell that will appear in the spell book
    /// </summary>
    public string SpellDescription
    {
        get { return spellDescription; }
    }
    /// <summary>
    /// This is the target transform for the spell
    /// </summary>
    public Transform SpellTarget
    {
        get;
        set;
    }
    /// <summary>
    /// This is the target position for the spell
    /// </summary>
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
        private set;
    }
    /// <summary>
    /// The position where the spell was started. Note this is not usually the entity
    /// more so the casting hand for example.
    /// </summary>
    public Transform SpellStartTransform
    {
        get;
        private set;
    }
    /// <summary>
    /// This is how much Elemental power it will cost to cast the spell
    /// </summary>
    public ElementalStats ElementalCost
    {
        get { return elementalCost; }
    }
    /// <summary>
    /// This is how long the spell can live for before it is destroyed
    /// </summary>
    public virtual float SpellLiveTime
    {
        get { return spellLiveTime; }
    }
    /// <summary>
    /// This is the type of the spell i.e. Area, Missile, Attached etc
    /// </summary>
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
    ///  This is the ID associated with the spell. This is used for hashmap look ups.
    /// </summary>
    public string SpellID
    {
        get { return spellID; }
    }
    /// <summary>
    /// The timer that willl control when the Spell should be destroyed. This ticks once the amount of time specified in spell
    /// live time has passed. This may be reset to prevent spell destruction.
    /// </summary>
    public Timer SpellDestroyTimer { get; set; }

    #endregion


    #region Events
    public event Action<Spell> OnSpellDestroy;
    #endregion

    /// <summary>
    /// This is called when a spell is first created. This is similiar to Awake but ensure it is called as the spell is created.
    /// </summary>
    public void InitializeSpell()
    {
        IgnoreEntities = new List<Entity>();
        gameObject.layer = 10;
        SpellEffectSetting = GetComponent<EffectSetting>();
        SpellEffectSetting.InitializeEffect();
        SpellDestroyTimer = new Timer(SpellLiveTime);
        // Stop the Timer until the spell is cast
        SpellDestroyTimer.IsStopped = true;
    }
    /// <summary>
    /// When a spell is created it must be cast before it can exist properly in the world. This will setup important information associated with the spell.
    /// </summary>
    /// <param name="castingEntity"></param>
    /// <param name="startPosition"></param>
    /// <param name="startVector"></param>
    /// <param name="spellTarget"></param>
    /// <param name="spellTargetPosition"></param>
    public void CastSpell(Entity castingEntity, Transform startPosition = null, Vector3? startVector = null, Transform spellTarget = null, Vector3? spellTargetPosition = null)
    {
        CastingEntity = castingEntity;

        SpellStartTransform = startPosition;
        SpellStartPosition = startVector.HasValue ? startVector.Value : startPosition.position;
        SpellTarget = spellTarget;
        SpellTargetPosition = spellTargetPosition;
        transform.position = SpellStartPosition;
        transform.rotation = CastingEntity.transform.rotation;

        // Reset and ensure the timer is started
        SpellDestroyTimer.Reset();
        SpellDestroyTimer.IsStopped = false;

        SpellEffectSetting.TriggerSpellCast();

        gameObject.SetActive(true);
    }

    #region Trigger Events

    /// <summary>
    /// Called when the spell should be destroyed. This calls the OnSpellDestoyEvent and will disable the spell script.
    /// </summary>
    public void DestroySpell()
    {
        SpellDestroyTimer.IsStopped = true;
        if (OnSpellDestroy != null)
            OnSpellDestroy(this);
        enabled = false;
    }

    /// <summary>
    /// This is triggered when the spell should be reset and returned to the pool.
    /// </summary>
    public void TriggerSpellReset()
    {
        OnSpellDestroy = null;
        IgnoreEntities.Clear();
    }

    #endregion

    private void Update()
    {
        if (SpellDestroyTimer.CanTick)
        {
            DestroySpell();
        }
    }

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