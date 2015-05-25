using UnityEngine;
using System.Collections;

public class EntityStatModifier : MonoBehaviour
{
    [SerializeField]
    private string _name = "NOTSET";
    [SerializeField]
    private string _id;
    [SerializeField]
    private Texture2D _icon;
    [SerializeField]
    private bool _stackable = false;
    [SerializeField]
    private bool _reapplyRefreshes = true;
    [SerializeField]
    private float _destroyTime;
    [SerializeField]
    private float _destroyTimeDelay;
    [SerializeField]
    private EntityStats _statModifiers;

    #region Properties

    public string ModifierName { get { return _name; } }
    public string ModifierID { get { return _id; } }
    public bool ReapplyRefreshes { get { return _reapplyRefreshes; } }
    public bool IsStackable { get { return _stackable; } }
    public Timer ModifierDestroyTimer { get; private set; }
    public Entity OwnerEntity { get; set; }
    public Spell CreatedBySpell { get; set; }

    #endregion

    public enum StatModifierDestroyEvent
    {
        Timed,
        Collision,
        SpellDestroy,
        EffectDestroy
    }

    /// <summary>
    /// This is called by the Entity when it has applied the Modifier to itself
    /// </summary>
    public void InitializeModifier(Entity entity)
    {
        OwnerEntity = entity;
        ModifierDestroyTimer = new Timer(_destroyTimeDelay);
    }

    public void TriggerDestroySpellModifier()
    {
        Invoke("DestroySpellModifier", _destroyTimeDelay);
    }

    private void DestroySpellModifier()
    {
        Destroy(gameObject);
    }

}
