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

    public EffectSetting SpellEffectSetting { get; set; }

    public SpellDeathMarker SpellDeathMarker
    {
        get { return _spellDeathMarker; }
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


    #region Events
    public event Action<Spell> OnSpellDestroy;
    #endregion


    public void Awake()
    {
    }

    public void Start()
    {

    }

    /// <summary>
    /// This is called when a spell is first created. This is similiar to Awake but ensure it is called as the spell is created.
    /// </summary>
    public void InitializeSpell()
    {
        IgnoreEntities = new List<Entity>();
        gameObject.layer = 10;
        SpellEffectSetting = GetComponent<EffectSetting>();
        SpellEffectSetting.InitializeEffect();
    }

    public void CastSpell(Entity castingEntity, Transform startPosition = null, Vector3? startVector = null, Transform spellTarget = null, Vector3? spellTargetPosition = null)
    {
        CastingEntity = castingEntity;

        SpellStartTransform = startPosition;
        SpellStartPosition = startVector.HasValue ? startVector.Value : startPosition.position;
        SpellTarget = spellTarget;
        SpellTargetPosition = spellTargetPosition;
        transform.position = SpellStartPosition;
        transform.rotation = CastingEntity.transform.rotation;
        OnSpellCast();
        Debug.Log("Active: " + gameObject.active);
        gameObject.SetActive(true);
    }

    #region Trigger Events

    /// <summary>
    /// Called when the spell is destroyed
    /// </summary>
    public void DestroySpell()
    {
        DestroyEvent();
        enabled = false;
    }

    /// <summary>
    /// Called when the spell is destroyed.
    /// </summary>
    public void DestroyEvent()
    {
        CancelInvoke();
        if (OnSpellDestroy != null)
            OnSpellDestroy(this);
    }

    public void TriggerSpellReset()
    {
        OnSpellDestroy = null;
        IgnoreEntities.Clear();
    }

    private IEnumerator TriggerSpellCast()
    {
        yield return null;
        OnSpellCast();
    }

    private void OnSpellCast()
    {
        Invoke("DestroySpell", SpellLiveTime);
        SpellEffectSetting.TriggerSpellCast();
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